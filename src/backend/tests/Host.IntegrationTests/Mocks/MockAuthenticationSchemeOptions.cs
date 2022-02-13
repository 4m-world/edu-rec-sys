using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Host.IntegrationTests.Mocks;

public class MockAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public Func<HttpContext, ClaimsPrincipal, Task> OnTokenValidated { get; set; } = (context, principal) => { return Task.CompletedTask; };
}
