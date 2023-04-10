using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Core.Interfaces;

namespace Core.Entities;

public class Product  : BaseEntity, IActive, IDelete
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(220)]
    public string NameArabic { get; set; }

    [Required]
    [MaxLength(220)]
    public string NameEnglish { get; set; }

    [Required]
    [MaxLength(400)]
    public string ShortDescriptionArabic { get; set; }

    [Required]
    [MaxLength(400)]
    public string ShortDescriptionEnglish { get; set; }

    public string DescriptionArabic { get; set; }
    public string DescriptionEnglish { get; set; }
    public decimal Price { get; set; }
    public int CountInStock { get; set; }

    [ForeignKey("SubCategoryId")]
    public Guid SubCategoryId { get; set; }
    public virtual SubCategory SubCategoryOfProduct { get; set; }
    public virtual ICollection<ProductImage> ImagesOfProduct { get; set; } = new HashSet<ProductImage>();
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
}
