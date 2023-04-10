using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Account  : IdentityUser
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? ResetPasswordCode { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordTokenExpiration { get; set; }
}
