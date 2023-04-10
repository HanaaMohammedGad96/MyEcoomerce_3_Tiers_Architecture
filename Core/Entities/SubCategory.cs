

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Core.Interfaces;

namespace Core.Entities;

public class SubCategory : BaseEntity , IActive
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string NameArabic { get; set; }

    [Required]
    [MaxLength(20)]
    public string NameEnglish { get; set; }

    [MaxLength(200)]
    public string DescriptionArabic { get; set; }

    [MaxLength(200)]
    public string DescriptionEnglish { get; set; }

    [Required(ErrorMessage = "Please choose category image")]
    public string ImagePath { get; set; }
    [ForeignKey("CategoryId")]
    public Guid CategoryId { get; set; }
    public virtual Category Category { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    public bool IsActive { get; set; } = true;
}
