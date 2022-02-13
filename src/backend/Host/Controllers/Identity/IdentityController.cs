using CodeMatrix.Mepd.Application.Identity.Users;
using CodeMatrix.Mepd.Application.Identity.Users.Password;
using CodeMatrix.Mepd.Application.Wrapper;

namespace CodeMatrix.Mepd.Host.Controllers.Identity;

/// <summary>
/// Identity controller
/// </summary>
public sealed class IdentityController : VersionNeutralApiController
{
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IUserService _userService;
    private string OriginFromRequest => Request.Headers.Origin.FirstOrDefault() ?? $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="identityService">Identity service</param>
    /// <param name="currentUser">Curent user service</param>
    /// <param name="userService">User service</param>
    public IdentityController(IIdentityService identityService, ICurrentUser currentUser, IUserService userService)
    {
        _identityService = identityService;
        _currentUser = currentUser;
        _userService = userService;
    }

    /// <summary>
    /// Register user account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    [MustHavePermission(MepdPermissions.Identity.Register)]
    public async Task<ActionResult<Result<string>>> RegisterAsync(RegisterUserRequest request)
    {
        return Ok(Result<string>.Success(await _identityService.RegisterAsync(request, OriginFromRequest)));
    }

    /// <summary>
    /// Confirm email
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="code">Confirmation code</param>
    /// <param name="tenant">Tenant</param>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Search))]
    public async Task<ActionResult<Result<string>>> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code, [FromQuery] string tenant)
    {
        return Ok(Result<string>.Success(await _identityService.ConfirmEmailAsync(userId, code, tenant)));
    }

    /// <summary>
    /// Confirm phone number
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="code">Confiramtion code</param>
    [HttpGet("confirm-phone-number")]
    [AllowAnonymous]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Search))]
    public async Task<ActionResult<Result<string>>> ConfirmPhoneNumberAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return Ok(Result<string>.Success(await _identityService.ConfirmPhoneNumberAsync(userId, code)));
    }

    /// <summary>
    /// Forgot password
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [TenantIdHeader]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Post))]
    public async Task<ActionResult<Result<string>>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        return Ok(Result<string>.Success(await _identityService.ForgotPasswordAsync(request, OriginFromRequest)));
    }

    /// <summary>
    /// Reset password
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [TenantIdHeader]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Post))]
    public async Task<ActionResult<Result<string>>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return Ok(Result<string>.Success(await _identityService.ResetPasswordAsync(request)));
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("profile")]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Put))]
    [MustHavePermission(MepdPermissions.UserProfile.Edit)]
    public  Task UpdateProfileAsync(UpdateProfileRequest request)
    {
        return _identityService.UpdateProfileAsync(request, _currentUser.GetUserId().ToString());
    }

    /// <summary>
    /// Get profile details
    /// </summary>
    [HttpGet("profile")]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Search))]
    [MustHavePermission(MepdPermissions.UserProfile.View)]
    public async Task<ActionResult<Result<UserDetailsDto>>> GetProfileDetailsAsync()
    {
        return Ok(Result<UserDetailsDto>.Success(await _userService.GetAsync(_currentUser.GetUserId().ToString())));
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("change-password")]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Put))]
    [MustHavePermission(MepdPermissions.UserProfile.ChangePassword)]
    public  Task<string> ChangePasswordAsync(ChangePasswordRequest model)
    {
        return _identityService.ChangePasswordAsync(model, _currentUser.GetUserId().ToString());
    }
}