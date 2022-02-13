using CodeMatrix.Mepd.Infrastructure.Identity;
using Host.IntegrationTests.Mocks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Host.IntegrationTests.Utils;

public static class HttpClientExtensions
{
    public static HttpClient SetFakeBearerToken(this HttpClient client, ApplicationUser user)
    {
        client.DefaultRequestHeaders.Authorization = (AuthenticationHeaderValue?)new AuthenticationHeaderValue(FakeJwtBearerDefaults.AuthenticationScheme, GenerateJwtToken(GetClaims(user, "localhost")));
        return client;
    }
}
