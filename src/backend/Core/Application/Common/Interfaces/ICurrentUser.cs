using System.Security.Claims;

namespace CodeMatrix.Mepd.Application.Common.Interfaces;

public interface ICurrentUser
{
    string Name { get; }

    string GetUserId();

    string GetUserEmail();

    string GetPartnerId();
    string GetTenant();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim> GetUserClaims();
}