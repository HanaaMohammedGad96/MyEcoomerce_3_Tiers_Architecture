using AutoMapper;
using Core;
using Core.Entities;
using Core.Interfaces;
using Data;
using Data.Repositories;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBaseRepository<Category> categoryRepository { get; private set; }
        public IBaseRepository<SubCategory> subCategoryRepository { get; private set; }
        public IBaseRepository<Product> productRepository { get; private set; }
        public IBaseRepository<ProductImage> productImagesRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context, IMapper mapper)
        {
            _context              = context;
            categoryRepository    = new BaseRepository<Category>(_context, mapper);
            subCategoryRepository = new BaseRepository<SubCategory>(_context, mapper);
            productRepository     = new BaseRepository<Product>(_context, mapper);
            productImagesRepository = new BaseRepository<ProductImage>(_context, mapper);
        }
        public int Complete()
            => _context.SaveChanges();
        public void Dispose()
            => _context.Dispose();
    }
}
