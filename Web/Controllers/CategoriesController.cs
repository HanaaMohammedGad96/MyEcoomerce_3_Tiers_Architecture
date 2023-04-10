using AutoMapper;
using BLL.Helpers;
using Core.Consts;
using Core.Interfaces;
using Core.Models.Attachment;
using Core.Models.Category;
using Core.Resources.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Exceptions;
using Core.Models;
using Core.Entities;
using Core;

namespace Web.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiVersion("1.0")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unit;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IWebHostEnvironment _environment;

    public CategoriesController(
        ICurrentUserService currentUser,
        IUnitOfWork unit, IMapper mapper,
        IWebHostEnvironment environment
        )
    {
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _unit = unit ?? throw new ArgumentNullException(nameof(unit));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("{id}", Name = "GetCategoryById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var category = await _unit.categoryRepository.GetByIdAsync(id);

        if (category == null)
            throw new NotFoundException(SharedRes.NotFound);

        return Ok(_mapper.Map<CategoryVm>(category));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoryVm>>> GetList()
    {
        var categories = await _unit.categoryRepository.GetListAsync();
        var result = _mapper.Map<IEnumerable<CategoryVm>>(categories);

        return Ok(result.Where(c => c.IsActive)
                        .OrderByDescending(c => c.CreatedDate)
                        .ToList());
    }

    #region Old implementation for GetPage
    /*
     [HttpGet]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryVm>>> GetPage(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize > maxSize)
            pageSize = maxSize;

        var (categories, paginationModel) = await _unit.categoryRepository.GetPageAsync(pageNumber, pageSize);

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationModel));

        if (!string.IsNullOrWhiteSpace(name))
        {
            name       = name.Trim();
            categories = categories.Where(c => c.NameArabic == name || c.NameEnglish == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            categories  = categories.Where(c => (c.NameArabic.Contains(searchQuery) || c.NameEnglish.Contains(searchQuery))
                               || (c.DescriptionArabic != null || c.DescriptionEnglish != null) 
                               && (c.DescriptionArabic.Contains(searchQuery)|| c.DescriptionEnglish.Contains(searchQuery)));
        }

        return Ok(_mapper.Map<IEnumerable<CategoryVm>>(categories));
    }
     */
    #endregion

    [HttpGet]
    //[Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagingVm<CategoryVm>>> GetPage(int pageIndex = 1, int pageSize = 10)
    {
        return Ok(_unit.categoryRepository.GetPageAsync<CategoryVm>("Id", false, pageIndex, pageSize));
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryVm>> Post([FromForm] PostCategoryDto model)
    {
        string wwwPath = _environment.WebRootPath;

        if (string.IsNullOrEmpty(wwwPath))
            throw new InternalServerException(SharedRes.InternalError);

        var path = await FileUploader.Upload(model?.CategoryImage, wwwPath);

        if (path == null)
            throw new InternalServerException(SharedRes.InternalError);

        var category = _mapper.Map<Category>(model);

        if (category == null)
            throw new NotFoundException(SharedRes.NotFound);

        category.ImagePath = path;
        if (_currentUser.IsAuthenticated)
            category.CreatedBy = _currentUser.UserId;

        category = await _unit.categoryRepository.AddAsync(category);

        int result = _unit.Complete();

        if (result == 0)
            return Problem();

        var createdCategory = _mapper.Map<CategoryVm>(category);

        return CreatedAtRoute("GetCategoryById", new { id = category.Id }, createdCategory);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Put(Guid id, [FromBody] PutCategoryDto model)
    {
        if (model == null)
            throw new NotFoundException(SharedRes.NotFound);

        var selectedCategory = await _unit.categoryRepository.GetByIdAsync(id);

        if (selectedCategory != null)
        {
            selectedCategory.NameArabic = model.NameArabic;
            selectedCategory.NameEnglish = model.NameEnglish;
            selectedCategory.DescriptionArabic = model.DescriptionArabic;
            selectedCategory.DescriptionEnglish = model.DescriptionEnglish;
            selectedCategory.IsActive = model.IsActive;
            selectedCategory.ModifiedDate = DateTime.Now;

            if (_currentUser.IsAuthenticated)
                selectedCategory.ModifiedBy = _currentUser.UserId;


            int result = _unit.Complete();

            if (result == 0)
                return Problem();

            return Ok(_mapper.Map<CategoryVm>(selectedCategory));
        }

        throw new NotFoundException(SharedRes.NotFound);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutImage(Guid id, [FromForm] AttachmentDto model)
    {
        if (model == null)
            throw new NotFoundException(SharedRes.NotFound);

        var selectedCategory = await _unit.categoryRepository.GetByIdAsync(id);

        if (selectedCategory != null)
        {
            string wwwPath = _environment.WebRootPath;

            if (string.IsNullOrEmpty(wwwPath))
                throw new InternalServerException(SharedRes.InternalError);

            var path = await FileUploader.Upload(model?.Image, wwwPath);

            if (path == null)
                throw new InternalServerException(SharedRes.InternalError);

            selectedCategory.ImagePath = path;
            selectedCategory.ModifiedDate = DateTime.Now;

            if (_currentUser.IsAuthenticated)
                selectedCategory.ModifiedBy = _currentUser.UserId;

            int result = _unit.Complete();

            if (result == 0)
                return Problem();

            return Ok(_mapper.Map<CategoryVm>(selectedCategory));
        }

        throw new NotFoundException(SharedRes.NotFound);
    }


    [Authorize(Roles = Roles.ADMIN)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var selectedCategory = await _unit.categoryRepository.GetByIdAsync(id);

        if (selectedCategory == null)
            throw new NotFoundException($"category with id = {id} is not found");

        _unit.categoryRepository.Delete(selectedCategory);

        var result = _unit.Complete();

        if (result == 0)
            throw new BadRequestException("delete proccess is not completed.");

        return NoContent();
    }
}
