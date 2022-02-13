using CodeMatrix.Mepd.Application.Identity.Tokens;
using CodeMatrix.Mepd.Application.Wrapper;
using CodeMatrix.Mepd.Infrastructure.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CodeMatrix.Mepd.Host.Controllers.Identity;

/// <summary>
/// Tokens controller
/// </summary>
public sealed class TokensController : VersionNeutralApiController
{
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Const.
    /// </summary>
    /// <param name="tokenService">Token service</param>
    public TokensController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Submit Credentials with Tenant Key to generate valid Access Token
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [TenantIdHeader]
    public async Task<ActionResult<Result<TokenResponse>>> GetTokenAsync(TokenRequest request)
    {
        var token = await _tokenService.GetTokenAsync(request, GenerateIPAddress());
        return Ok(Result<TokenResponse>.Success(token));
    }

    /// <summary>
    /// Submit a valid refresh token with Tenant Key to regenerate valid Access Token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [TenantIdHeader]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Post))]
    public async Task<TokenResponse> RefreshAsync(RefreshTokenRequest request)
    {
        var response = await _tokenService.RefreshTokenAsync(request, GenerateIPAddress());
        return response;
    }

    //[HttpPost("revoke-token")]
    //[TenantIdHeader]
    //[ApiConventionMethod(typeof (MepdApiConventions), nameof(MepdApiConventions.Post))]
    //public  async Task RevokeAsync()

    private string GenerateIPAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"];
        }
        else
        {
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
        }
    }
}