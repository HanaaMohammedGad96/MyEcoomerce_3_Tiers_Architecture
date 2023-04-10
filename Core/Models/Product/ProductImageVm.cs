namespace Core.Models.Product;

public class ProductImageVm
{
    public Guid Id { get; set; }
    public string ImageName { get; set; }
    public string ImagePath { get; set; }
    public bool IsDefault { get; set; }
}
