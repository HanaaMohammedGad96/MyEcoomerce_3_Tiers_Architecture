using Core.Entities;
using Microsoft.AspNetCore.Http;

namespace BLL.Helpers;

public class FileUploader
{
    public static async Task<string> Upload(IFormFile file, string webRootPath)
    {
        if (file == null)
            throw new ArgumentNullException();
        if (file.FileName == null || file.FileName.Length == 0)
            throw new ArgumentNullException();

        var path = Path.Combine(webRootPath, "Images/", file.FileName);

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
            stream.Close();
        }

        return path;
    }

    public static async Task<ProductImage> UpdateProductImage(ProductImage productImage, IFormFile image, string webRootPath)
    {
        if (image == null)
            throw new ArgumentNullException();

        if (image.FileName == null || image.FileName.Length == 0)
            throw new ArgumentNullException();

        var path = Path.Combine(webRootPath, "Images/", image.FileName);

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await image.CopyToAsync(stream);
            stream.Close();
        }
        productImage.ImageName = image.FileName;
        productImage.ImagePath = path;
        
        return productImage;
    }

    public static async Task<IEnumerable<ProductImage>> UploadProductImages(Guid productId, List<IFormFile> images, string webRootPath)
    {
        List<ProductImage> productImages = new List<ProductImage>();

        if (images == null)
            throw new ArgumentNullException();

        foreach (var image in images)
        {
            if (image.FileName == null || image.FileName.Length == 0)
                throw new ArgumentNullException();

            var path = Path.Combine(webRootPath, "Images/", image.FileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
                stream.Close();
            }

            productImages.Add(new ProductImage(productId, image.FileName, path));
        }

        return productImages;
    }
}
