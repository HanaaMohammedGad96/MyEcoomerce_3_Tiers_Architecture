namespace Core.Interfaces;

public interface ICurrentUserService
{
    public bool IsAuthenticated { get; }
    public bool IsAdmin { get; }
    //public IList<Role> Roles { get; }
    //public string Role { get; }

    public Guid UserId { get; }
    public string Username { get; }
    public string Email { get; }
    public bool IsUser { get; }

}
