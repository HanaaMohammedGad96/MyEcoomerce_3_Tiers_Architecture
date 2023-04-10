namespace Core.Models.Product;

public class ProductVm
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string MainImage { get; set; }
    public decimal Price { get; set; }
    public int CountInStock { get; set; }
    public string Category { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
