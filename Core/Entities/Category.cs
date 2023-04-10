
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Core.Interfaces;

namespace Core.Entities;

public class Category : BaseEntity, IActive
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
    public virtual ICollection<SubCategory> SubCategories { get; set; } = new HashSet<SubCategory>();
    public bool IsActive { get; set; } = true;
}
