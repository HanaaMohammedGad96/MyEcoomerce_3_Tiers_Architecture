using Core.Entities;
using Core.Interfaces;

namespace Core;

public interface IUnitOfWork : IDisposable
{
    IBaseRepository<Category> categoryRepository { get; }
    IBaseRepository<SubCategory> subCategoryRepository { get; }
    IBaseRepository<Product> productRepository { get; }
    IBaseRepository<ProductImage> productImagesRepository { get; }
    int Complete();
}
