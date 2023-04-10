using Core.Models.Account;

namespace Core.Interfaces;

public interface IAuthRepository
{
    Task<AuthResponse> RegisterAsync(RegisterationDto model);
    Task<AuthResponse> LoginAsync(LoginingDto model);
    //Task<AuthResponse> Logout();
    Task<int> GenerateResetPassordOTP(string email);
    Task<LoginingDto> ResetPassword(ResetPasswordDto model);
    Task<string> AddRoleAsync(AddRoleModel model);
}
