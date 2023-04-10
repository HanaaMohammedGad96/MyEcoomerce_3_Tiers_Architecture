using AutoMapper;
using BLL.Helpers;
using Core;
using Core.Consts;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models.Product;
using Core.Resources.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiVersion("1.0")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _unit;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IWebHostEnvironment _environment;
    const int maxSize = 10;
    public ProductsController(
        ICurrentUserService currentUser,
        IUnitOfWork unit, IMapper mapper, 
        IWebHostEnvironment environment)
    {
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _unit = unit ?? throw new ArgumentNullException(nameof(unit));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    [HttpGet]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductVm>>> GetPage(int pageIndex = 1, int pageSize = 10)
    {
       return Ok(_unit.productRepository.GetPageAsync<ProductVm>("Id", false, pageIndex, pageSize));
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("{id}", Name = "GetEditableProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEditableProduct(Guid id)
    {
        var selectedProduct = await _unit.productRepository
                              .Get(p => p.Id == id)
                              .AsNoTracking()
                              .Include(p => p.ImagesOfProduct)
                              .FirstOrDefaultAsync();

        if (selectedProduct is null)
            throw new NotFoundException(SharedRes.NotFound);

        return Ok(_mapper.Map<PutProductDto>(selectedProduct));
    }

    [HttpPost]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductVm>> Post([FromForm] PostProductDto model)
    {
        var product = new Product()
        {
            NameArabic = model.NameArabic,
            NameEnglish = model.NameEnglish,
            ShortDescriptionArabic = model.ShortDescriptionArabic,
            ShortDescriptionEnglish = model.ShortDescriptionEnglish,
            DescriptionArabic = model.DescriptionArabic,
            DescriptionEnglish = model.DescriptionEnglish,
            Price = model.Price,
            CountInStock = model.CountInStock,
            SubCategoryId = model.SubCategoryId,
            IsActive = true
        };

        if (product is null)
            throw new NotFoundException(SharedRes.NotFound);

        if (_currentUser.IsAuthenticated)
            product.CreatedBy = _currentUser.UserId;

        product = await _unit.productRepository.AddAsync(product);

        string wwwPath = _environment.WebRootPath;

        if (string.IsNullOrEmpty(wwwPath))
            throw new InternalServerException(SharedRes.InternalError);

        if (model.ImagesOfProduct is null)
            throw new BadRequestException("you must enter at least image.");

        var images = await FileUploader.UploadProductImages(product.Id, model.ImagesOfProduct, wwwPath);
        var mainImage = images.FirstOrDefault();
        mainImage.IsDefault = true;

        await _unit.productImagesRepository.AddRangeAsync(images);

        int result = _unit.Complete();

        if (result == 0)
            return Problem();

        var createdProduct = _mapper.Map<ProductVm>(product);
        createdProduct.MainImage = mainImage.ImagePath;

        return Ok(createdProduct);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Put(Guid id, [FromForm] PutProductDto model)
    {
        if (model is null)
            throw new NotFoundException(SharedRes.NotFound);

        var updatedProduct = await _unit.productRepository.GetByIdAsync(id);

        if (updatedProduct is not null)
        {
            updatedProduct.NameArabic = model.NameArabic;
            updatedProduct.NameEnglish = model.NameEnglish;
            updatedProduct.ShortDescriptionArabic = model.ShortDescriptionArabic;
            updatedProduct.ShortDescriptionEnglish = model.ShortDescriptionEnglish;
            updatedProduct.DescriptionArabic = model.DescriptionArabic;
            updatedProduct.DescriptionEnglish = model.DescriptionEnglish;
            updatedProduct.Price = model.Price;
            updatedProduct.CountInStock = model.CountInStock;
            updatedProduct.SubCategoryId = model.SubCategoryId;

            if (model.ImagesOfProduct is not null)
            {
                string wwwPath = _environment.WebRootPath;

                if (string.IsNullOrEmpty(wwwPath))
                    throw new InternalServerException(SharedRes.InternalError);

                var images = await FileUploader.UploadProductImages(updatedProduct.Id, model.ImagesOfProduct, wwwPath);

                //if (model.StoredImagesOfProduct is null)
                //{
                //    var mainImage = images.FirstOrDefault();
                //    mainImage.IsDefault = true;
                //}
                await _unit.productImagesRepository.AddRangeAsync(images);
            }

            int result = _unit.Complete();

            if (result == 0)
                return Problem();

            return Ok(updatedProduct);

        }
        throw new NotFoundException(SharedRes.NotFound); 
    }

    [HttpPut("{productId}")]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutProductImage(Guid productId, [FromForm] ProductImageDto model)
    {
        var productImage = await _unit.productImagesRepository
                        .Get(p => p.Id == model.Id && p.ProductId == productId)
                        .FirstOrDefaultAsync();

        if (productImage is null)
            throw new NotFoundException(SharedRes.NotFound);

        string wwwPath = _environment.WebRootPath;

        if (string.IsNullOrEmpty(wwwPath))
            throw new InternalServerException(SharedRes.InternalError);

        productImage = await FileUploader.UpdateProductImage(productImage, model.Image, wwwPath);

        int result = _unit.Complete();

        if (result == 0)
            return Problem();

        return Ok(_mapper.Map<ProductImageVm>(productImage));
    }

    [HttpPut]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ChangeMainImage([FromQuery] Guid productId, [FromQuery] Guid id)
    {
        var mainImage = await _unit.productImagesRepository
                     .Get(i => i.ProductId == productId && i.IsDefault)
                     .FirstOrDefaultAsync();
        mainImage.IsDefault = false;

        var updatedMainImage = await _unit.productImagesRepository
                    .Get(i => i.ProductId == productId && i.Id == id)
                    .FirstOrDefaultAsync();

        updatedMainImage.IsDefault = true;

        int result = _unit.Complete();

        if (result == 0)
            return Problem();

        return Ok(_mapper.Map<ProductImageVm>(updatedMainImage));
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var selectedProduct = await _unit.productRepository.GetByIdAsync(id);
        if (selectedProduct is null)
            throw new NotFoundException(SharedRes.NotFound);

        _unit.productRepository.Delete(selectedProduct);

        var result = _unit.Complete();

        if (result == 0)
            throw new BadRequestException(SharedRes.InternalError);

        return NoContent();
    }
}
