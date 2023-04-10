using Core.Consts;
using Core.Enums;
using Core.Interfaces;
using System.Security.Claims;
using Core.Exceptions;

namespace Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
      =>  _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;
    public bool IsAuthenticated => User.Identity.IsAuthenticated;
    public Guid UserId => new Guid(User.FindFirstValue("uid"));
    public string Username => User.FindFirstValue(ClaimTypes.NameIdentifier);
    public string Email => User.FindFirstValue(ClaimTypes.Email);
    public bool IsAdmin => User.IsInRole(Roles.ADMIN);
    public bool IsUser => User.IsInRole(Roles.USER);

    public AppType AppType
    {
        get
        {
            var header = _httpContextAccessor.HttpContext?.Request.Headers["Application-Type"].ToString();

            if (string.IsNullOrEmpty(header)) throw new BadRequestException("Can't detect the Application-Type");

            var succeeded = Enum.TryParse<AppType>(header, out var appType);

            if (!succeeded) throw new BadRequestException("Can't detect the Application-Type");

            return appType;
        }
    }

}
