using AutoMapper;
using BLL.Helpers;
using Core.Consts;
using Core.Models.Attachment;
using Core.Models.SubCategory;
using Core.Resources.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Core;
using Core.Models.Category;
using Core.Entities;

namespace Web.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SubCategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly IWebHostEnvironment _environment;
        const int maxSize = 10;
        public SubCategoriesController(ICurrentUserService currentUser, IUnitOfWork unit, IMapper mapper, IWebHostEnvironment environment)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _unit        = unit ?? throw new ArgumentNullException(nameof(unit));
            _mapper      = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        [HttpGet("{id}", Name = "GetSubCategoryByid")]
        [Authorize(Roles = Roles.ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSubCategory(Guid id)
        {
            var subCategory = await  _unit.subCategoryRepository
                             .Get(s => s.Id == id)
                             .Include(s => s.Category)
                             .FirstOrDefaultAsync();

            if (subCategory == null)
                throw new NotFoundException(SharedRes.NotFound);
            return Ok(_mapper.Map<SubCategoryVm>(subCategory));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SubCategoryVm>>> GetList()
        {
            var categories = await _unit.subCategoryRepository.GetListAsync();

            var result = _mapper.Map<IEnumerable<SubCategoryVm>>(categories);

            return Ok(result.Where(sub => sub.IsActive)
                .OrderByDescending(sub => sub.CreatedDate)
                .ToList());
        }

        [HttpGet]
        [Authorize(Roles = Roles.ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SubCategoryVm>>> GetPage(int pageIndex = 1, int pageSize = 10)
        {
            return Ok(_unit.subCategoryRepository.GetPageAsync<SubCategoryVm>("Id", false, pageIndex, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = Roles.ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryVm>> Post([FromForm] PostSubCategoryDto model)
        {
            string wwwPath = _environment.WebRootPath;

            if (string.IsNullOrEmpty(wwwPath))
                throw new InternalServerException(SharedRes.InternalError);

            var path = await FileUploader.Upload(model?.SubCategoryImage, wwwPath);

            if (path == null)
                throw new InternalServerException(SharedRes.InternalError);

            var subCategory = _mapper.Map<SubCategory>(model);

            if (subCategory == null)
                throw new NotFoundException(SharedRes.NotFound);

            if (_currentUser.IsAuthenticated)
                subCategory.CreatedBy = _currentUser.UserId;

            subCategory.ImagePath = path;

            subCategory = await _unit.subCategoryRepository.AddAsync(subCategory);

            int result = _unit.Complete();

            if (result == 0)
                return Problem();

            var createdSubCategory = _mapper.Map<SubCategoryVm>(subCategory);

            return CreatedAtRoute("GetSubCategoryById", new { id = subCategory.Id }, createdSubCategory);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Guid id, [FromBody] PutSubCategoryDto model)
        {
            if (model == null)
                throw new NotFoundException(SharedRes.NotFound);

            var selectedSubCategory = await _unit.subCategoryRepository.GetByIdAsync(id);
           
            if (selectedSubCategory != null)
            {
                selectedSubCategory.CategoryId         = model.CategoryId;
                selectedSubCategory.NameArabic         = model.NameArabic;
                selectedSubCategory.NameEnglish        = model.NameEnglish;
                selectedSubCategory.DescriptionArabic  = model.DescriptionArabic;
                selectedSubCategory.DescriptionEnglish = model.DescriptionEnglish;
                selectedSubCategory.ModifiedDate       = DateTime.Now;
                selectedSubCategory.IsActive           = model.IsActive;

                if (_currentUser.IsAuthenticated)
                    selectedSubCategory.ModifiedBy = _currentUser.UserId;

                int result = _unit.Complete();

                if (result == 0)
                    return Problem();

                return Ok(_mapper.Map<SubCategoryVm>(selectedSubCategory));
            }

            throw new NotFoundException(SharedRes.NotFound);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PutImage(Guid id, [FromForm] AttachmentDto model)
        {
            if (model == null)
                throw new NotFoundException(SharedRes.NotFound);

            var selectedSubCategory = await _unit.subCategoryRepository.GetByIdAsync(id);

            if (selectedSubCategory != null)
            {
                string wwwPath = _environment.WebRootPath;

                if (string.IsNullOrEmpty(wwwPath))
                    throw new InternalServerException(SharedRes.InternalError);

                var path = await FileUploader.Upload(model?.Image, wwwPath);

                if (path == null)
                    throw new InternalServerException(SharedRes.InternalError);

                selectedSubCategory.ImagePath = path;
                selectedSubCategory.ModifiedDate = DateTime.Now;

                if (_currentUser.IsAuthenticated)
                    selectedSubCategory.ModifiedBy = _currentUser.UserId;

                int result = _unit.Complete();

                if (result == 0)
                    return Problem();

                return Ok(_mapper.Map<SubCategoryVm>(selectedSubCategory));
            }

            throw new NotFoundException(SharedRes.NotFound);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var selectedSubCategory = await _unit.subCategoryRepository.GetByIdAsync(id);

            if (selectedSubCategory == null)
                throw new NotFoundException(SharedRes.NotFound);

            _unit.subCategoryRepository.Delete(selectedSubCategory);

            var result = _unit.Complete();

            if (result == 0)
                throw new BadRequestException(SharedRes.InternalError);

            return NoContent();
        }
    }
}
