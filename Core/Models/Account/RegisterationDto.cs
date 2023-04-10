
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Account;

public class RegisterationDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;
    [Required, StringLength(128)]
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(256)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(13)]
    public string PhoneNumber { get; set; } = string.Empty;
}
