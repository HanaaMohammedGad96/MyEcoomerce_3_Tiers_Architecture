using System.ComponentModel.DataAnnotations;

namespace Core.Models.Account;

public class AddRoleModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required] 
    public string Role { get; set; } = string.Empty;
}
