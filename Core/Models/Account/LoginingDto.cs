using System.ComponentModel.DataAnnotations;

namespace Core.Models.Account;

public class LoginingDto
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
