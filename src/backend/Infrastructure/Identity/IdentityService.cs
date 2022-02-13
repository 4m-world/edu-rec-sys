using CodeMatrix.Mepd.Application.Common.Exceptions;
using CodeMatrix.Mepd.Application.Common.FileStorage;
using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Application.Common.Mailing;
using CodeMatrix.Mepd.Application.Identity.Users;
using CodeMatrix.Mepd.Application.Identity.Users.Password;
using CodeMatrix.Mepd.Infrastructure.Common;
using CodeMatrix.Mepd.Infrastructure.Mailing;
using CodeMatrix.Mepd.Shared.DTOs.Common;
using CodeMatrix.Mepd.Shared.Multitenancy;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Text;

namespace CodeMatrix.Mepd.Infrastructure.Identity;

/// <inheritdoc />
public class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;
    private readonly MailSettings _mailSettings;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private readonly ITenantInfo _currentTenant;
    private readonly IFileStorageService _fileStorage;
    private readonly IEmailTemplateService _templateService;
    private readonly IUserService _userService;

    /// <summary>
    /// Initalize identity service
    /// </summary>
    /// <param name="signInManager">Sign in manager</param>
    /// <param name="userManager">User manager</param>
    /// <param name="roleManager">Role manager</param>
    /// <param name="jobService">Job service</param>
    /// <param name="mailService">Mail service</param>
    /// <param name="mailSettings">Mail settings</param>
    /// <param name="localizer">String localizer</param>
    /// <param name="currentTenant">Tenat service</param>
    /// <param name="fileStorage">Filer storage</param>
    /// <param name="templateService">Template service</param>
    /// <param name="userService">USer service</param>
    public IdentityService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IJobService jobService,
        IMailService mailService,
        IOptions<MailSettings> mailSettings,
        IStringLocalizer<IdentityService> localizer,
        ITenantInfo currentTenant,
        IFileStorageService fileStorage,
        IEmailTemplateService templateService,
        IUserService userService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _jobService = jobService;
        _mailService = mailService;
        _mailSettings = mailSettings.Value;
        _localizer = localizer;
        _currentTenant = currentTenant;
        _fileStorage = fileStorage;
        _templateService = templateService;
        _userService = userService;
    }

    /// <inheritdoc />
    public async Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal)
    {
        string objectId = principal.GetObjectId();
        if (string.IsNullOrWhiteSpace(objectId))
        {
            throw new InternalServerException(_localizer["Invalid objectId"]);
        }

        var user = await _userManager.Users.Where(u => u.ObjectId == objectId).FirstOrDefaultAsync();
        if (user is null)
        {
            user = await CreateOrUpdateFromPrincipalAsync(principal);
        }

        // Add user to incoming role if that isn't the case yet
        if (principal.FindFirstValue(ClaimTypes.Role) is string role &&
            await _roleManager.RoleExistsAsync(role) &&
            !await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return user.Id;
    }

    /// <inheritdoc />
    private async Task<ApplicationUser> CreateOrUpdateFromPrincipalAsync(ClaimsPrincipal principal)
    {
        string email = principal.FindFirstValue(ClaimTypes.Upn);
        string username = principal.GetDisplayName();
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username))
        {
            throw new InternalServerException(string.Format(_localizer["Username or Email not valid."]));
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
        {
            throw new InternalServerException(string.Format(_localizer["Username {0} is already taken."], username));
        }

        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(email);
            if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
            {
                throw new InternalServerException(string.Format(_localizer["Email {0} is already taken."], email));
            }
        }

        IdentityResult result;
        if (user is not null)
        {
            user.ObjectId = principal.GetObjectId();
            result = await _userManager.UpdateAsync(user);
        }
        else
        {
            user = new ApplicationUser
            {
                ObjectId = principal.GetObjectId(),
                FirstName = principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = principal.FindFirstValue(ClaimTypes.Surname),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            result = await _userManager.CreateAsync(user);
        }

        if (!result.Succeeded)
        {
            throw new InternalServerException(_localizer["Validation Errors Occurred."], result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }

        return user;
    }

    /// <inheritdoc />
    public async Task<string> RegisterAsync(RegisterUserRequest request, string origin)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InternalServerException(_localizer["Validation Errors Occurred."], result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }

        await _userManager.AddToRoleAsync(user, MepdRoles.Basic);

        var messages = new List<string> { string.Format(_localizer["User {0} Registered."], user.UserName) };

        if (_mailSettings.EnableVerification && !string.IsNullOrEmpty(user.Email))
        {
            // send verification email
            string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
            var mailRequest = new MailRequest(
                new List<string> { user.Email },
                _localizer["Confirm Registration"],
                _templateService.GenerateEmailConfirmationMail(user.UserName ?? "User", user.Email, emailVerificationUri));
            _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
            messages.Add(_localizer[$"Please check {user.Email} to verify your account!"]);
        }

        return string.Join(Environment.NewLine, messages);
    }

    /// <inheritdoc />
    public async Task<string> ConfirmEmailAsync(string userId, string code, string tenant)
    {
        var user = await _userManager.Users
            .Where(u => u.Id == userId && !u.EmailConfirmed)
            .FirstOrDefaultAsync();

        _ = user ?? throw new InternalServerException(_localizer["An error occurred while confirming E-Mail."]);


        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded
            ? string.Format(_localizer["Account Confirmed for E-Mail {0}."], user.Email)
            : throw new InternalServerException(string.Format(_localizer["An error occurred while confirming {0}"], user.Email));
    }

    /// <inheritdoc />
    public async Task<string> ConfirmPhoneNumberAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new InternalServerException(_localizer["An error occurred while confirming Mobile Phone."]);

        var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code);

        return result.Succeeded
            ? user.EmailConfirmed
                ? string.Format(_localizer["Account Confirmed for Phone Number {0}. You can now use the /api/identity/token endpoint to generate JWT."], user.PhoneNumber)
                : string.Format(_localizer["Account Confirmed for Phone Number {0}. You should confirm your E-mail before using the /api/identity/token endpoint to generate JWT."], user.PhoneNumber)
            : throw new InternalServerException(string.Format(_localizer["An error occurred while confirming {0}"], user.PhoneNumber));
    }

    /// <inheritdoc />
    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Normalize());
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            throw new InternalServerException(_localizer["An Error has occurred!"]);
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        const string route = "reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);

        var templateProps = new Dictionary<string, string>
        {
            { "UserName", user.UserName },
            { "Token", code  },
            { "EndPointUri", endpointUri.ToString() }
        };

        var mailRequest = new MailRequest(
            new List<string> { request.Email },
            _localizer["Reset Password"],
            _templateService.GenerateEmailFromTemlateMail(templateProps, "reset-password"));

        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
        return _localizer["Password Reset Mail has been sent to your authorized Email."];
    }

    /// <inheritdoc />
    public async Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email?.Normalize());

        // Don't reveal that the user does not exist
        _ = user ?? throw new InternalServerException(_localizer["An Error has occurred!"]);

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

        return result.Succeeded
            ? _localizer["Password Reset Successful!"]
            : throw new InternalServerException(_localizer["An Error has occurred!"]);
    }

    /// <inheritdoc />
    public async Task UpdateProfileAsync(UpdateProfileRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        string currentImage = user.ImageUrl ?? string.Empty;
        if (request.Image != null)
        {
            user.ImageUrl = await _fileStorage.UploadAsync<ApplicationUser>(request.Image, FileType.Image);
            if (!string.IsNullOrEmpty(currentImage))
            {
                string root = Directory.GetCurrentDirectory();
                string filePath = currentImage.Replace("{server_url}/", string.Empty, StringComparison.OrdinalIgnoreCase);
                _fileStorage.Remove(Path.Combine(root, filePath));
            }
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        string phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (request.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
        }

        var identityResult = await _userManager.UpdateAsync(user);

        await _signInManager.RefreshSignInAsync(user);

        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
            throw new InternalServerException(_localizer["Update profile failed"], errors);
        }

    }

    /// <inheritdoc />
    public async Task<string> ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        _ = user ?? throw new NotFoundException(_localizer["identity.usernotfound"]);

        var identityResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
            throw new InternalServerException(_localizer["changePassword.failed"], errors);
        }

        return _localizer["changePassword.success"];
    }

    private async Task<string> GetEmailVerificationUriAsync(ApplicationUser user, string origin)
    {
        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        const string route = "confirm-email";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), QueryStringKeys.UserId, user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, QueryStringKeys.Code, code);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, MultitenancyConstants.TenantIdName, _currentTenant.Id);
        return verificationUri;
    }
}