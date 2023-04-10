
using Core.Consts;
using Core.Entities;
using Core.Exceptions;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.Account;
using Core.Resources.Account;
using Core.Resources.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Data.Repositories;

public class AuthRepository : IAuthRepository
{
    private UserManager<Account>? _userManager;
    private RoleManager<IdentityRole>? _roleManager;
    private JWT? _jwt;

    public AuthRepository(UserManager<Account> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwt = jwt.Value;
    }

    public async Task<string> AddRoleAsync(AddRoleModel model)
    {
        var account = await _userManager.FindByIdAsync(model.UserId);

        if (account is null || !await _roleManager.RoleExistsAsync(model.Role))
            return AccountRes.NotValidCredentials;

        if (await _userManager.IsInRoleAsync(account, model.Role))
            return AccountRes.AlreadyAssignForRole;

        var result = await _userManager.AddToRoleAsync(account, model.Role);

        return result.Succeeded ? string.Empty : SharedRes.Error;
    }

    public async Task<AuthResponse> LoginAsync(LoginingDto model)
    {
        var authResponse = new AuthResponse();
        var account      = await _userManager.FindByEmailAsync(model.Email);

        if (account is null || !await _userManager.CheckPasswordAsync(account, model.Password)) 
        {
            authResponse.Message = AccountRes.NotValidCredentials;
            return authResponse;
        }

        var userRoles        = await _userManager.GetRolesAsync(account);
        var jwtSecurityToken = await CreateJwt(account);

        authResponse.Message        = AccountRes.SuccessLogin; 
        authResponse.Email          = account.Email; 
        authResponse.Username       = account.UserName;
        authResponse.ExpireIn       = jwtSecurityToken.ValidTo;
        authResponse.Roles          = userRoles.ToList();
        authResponse.Token          = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authResponse.IsAuthenticate = true;

        return authResponse;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterationDto model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) is not null)
            return new AuthResponse { Message = AccountRes.AlreadyExist, IsAuthenticate = false };
        if (await _userManager.FindByNameAsync(model.Username) is not null)
            return new AuthResponse { Message = AccountRes.AlreadyExist, IsAuthenticate = false };
       
        var account = new Account
        {
            Name        = model.Name,
            UserName    = model.Username,
            PhoneNumber = model.PhoneNumber,
            Email       = model.Email,
        };

        var result = await _userManager.CreateAsync(account, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Empty;

            foreach (var error in result.Errors)
                errors += $"{error.Description}";

            return new AuthResponse { Message = errors, IsAuthenticate = false };
        }

        await _userManager.AddToRoleAsync(account, Roles.USER);

        var jwtSecurityToken = await CreateJwt(account);

        return new AuthResponse 
        {
            Email          = account.Email,
            ExpireIn       = jwtSecurityToken.ValidTo,
            IsAuthenticate = true,
            Roles          = new List<string> { Roles.USER },
            Token          = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Username       = account.UserName
        };
    }

    private async Task<JwtSecurityToken> CreateJwt(Account user) 
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var userRoles  = await _userManager.GetRolesAsync(user); 

        var roleClaims = new List<Claim>();

        foreach (var role in userRoles)
            roleClaims.Add(new Claim(ClaimTypes.Role, role));

        var authClaims = new List<Claim>
        {
          new Claim(JwtRegisteredClaimNames.Email, user.Email),
          new Claim(JwtRegisteredClaimNames.Sub,   user.UserName),
          new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
          new Claim("uid", user.Id),
        }
        .Union(userClaims)
        .Union(roleClaims);

        var authSigningKey       = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var signingCredentials   = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);
        
        var token     = new JwtSecurityToken
            (
                issuer:     _jwt.Issuer,
                audience:   _jwt.Audience,
                claims:     authClaims,
                expires:    DateTime.Now.AddHours(_jwt.ExpireIn),
                signingCredentials: signingCredentials
            );

        return token;
    }

    public async Task<int> GenerateResetPassordOTP(string email)
    {
        #region another way
        //var user = await _userManager.FindByEmailAsync(email);

        //if (user is null)
        //    throw new NotFoundException("this user is notfound");

        //string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //code = HttpUtility.UrlEncode(code);
        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //return code;
        //string code = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        #endregion

        Random rnd = new Random();
        int code   = rnd.Next();
        var user   = await _userManager.FindByEmailAsync(email);

        if (user is null)
            throw new NotFoundException(AccountRes.UserNotFound);

        user.ResetPasswordCode            = code;
        user.ResetPasswordToken           = await _userManager.GeneratePasswordResetTokenAsync(user);
        user.ResetPasswordTokenExpiration = DateTime.Now.AddMinutes(10);

        await _userManager.UpdateAsync(user);  

        return code;
    }

    public async Task<LoginingDto> ResetPassword(ResetPasswordDto model) 
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        
        var resetPasswordCode = user.ResetPasswordCode;

        if (resetPasswordCode is null || resetPasswordCode != model.Token)
            throw new BadRequestException(AccountRes.InvalidToken);

        if (user.ResetPasswordTokenExpiration < DateTime.Now)
            throw new BadRequestException(AccountRes.ExpiredToken);

        var result = await _userManager.ResetPasswordAsync(user, user.ResetPasswordToken, model.NewPassword);

        if (!result.Succeeded)
            throw new BadRequestException(SharedRes.Error);

        LoginingDto credentials = new LoginingDto
        {
            Email    = model.Email,
            Password = model.NewPassword
        };

        return credentials;
    }
}
