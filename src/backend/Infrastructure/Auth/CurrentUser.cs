using CodeMatrix.Mepd.Application.Common.Interfaces;
using System.Security.Claims;

namespace CodeMatrix.Mepd.Infrastructure.Auth;

public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    private ClaimsPrincipal _user;

    public string Name => _user?.Identity?.Name;

    private string _userId = Guid.Empty.ToString();

    public string GetUserId() =>
        IsAuthenticated()
            ? Guid.Parse(_user?.GetUserId() ?? Guid.Empty.ToString()).ToString()
            : _userId;

    public string GetUserEmail() =>
        IsAuthenticated()
            ? _user!.GetEmail()
            : string.Empty;

    public string GetPartnerId() =>
        IsAuthenticated()
            ? _user!.GetPartnerId()
            : string.Empty;

    public bool IsAuthenticated() =>
        _user?.Identity?.IsAuthenticated is true;

    public bool IsInRole(string role) =>
        _user?.IsInRole(role) is true;

    public IEnumerable<Claim> GetUserClaims() =>
        _user?.Claims;

    public string GetTenant() =>
        IsAuthenticated() ? _user?.GetTenant() : string.Empty;

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        _user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        // Todo: Think about user migration from the old system
        if (_userId != Guid.Empty.ToString())
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        if (!string.IsNullOrEmpty(userId))
        {
            _userId = Guid.Parse(userId).ToString();
        }
    }
}