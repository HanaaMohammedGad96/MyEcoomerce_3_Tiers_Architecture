namespace Core.Models.Account;

public class ResetPasswordDto
{
    public string Email { get; set; }
    public int Token { get; set; }
    public string NewPassword { get; set; }
}
