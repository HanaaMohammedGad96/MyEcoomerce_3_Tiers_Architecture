using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class ProductImage :BaseEntity
{
    public ProductImage() {}
    public ProductImage(Guid productId, string name, string path, bool isDefault = false)
    {
        ProductId = productId;
        ImageName = name;
        ImagePath = path;
        IsDefault = isDefault;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Please enter the path of the image")]
    public string ImagePath { get; set; }

    [Required(ErrorMessage = "Please enter the name of the image")]
    public string ImageName { get; set; }
    public bool IsDefault { get; set; }

    [ForeignKey("ProductId")]
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }
}
