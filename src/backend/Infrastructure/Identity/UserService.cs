using Ardalis.Specification.EntityFrameworkCore;
using CodeMatrix.Mepd.Application.Common.Exceptions;
using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Application.Common.Mailing;
using CodeMatrix.Mepd.Application.Common.Models;
using CodeMatrix.Mepd.Application.Common.Specifications;
using CodeMatrix.Mepd.Application.Identity.Roles;
using CodeMatrix.Mepd.Application.Identity.Users;
using CodeMatrix.Mepd.Infrastructure.Common;
using CodeMatrix.Mepd.Infrastructure.Mailing;
using CodeMatrix.Mepd.Infrastructure.Multitenancy;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using CodeMatrix.Mepd.Shared.Authorization;
using CodeMatrix.Mepd.Shared.Multitenancy;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;
using System.Text;

namespace CodeMatrix.Mepd.Infrastructure.Identity;

/// <inheritdoc />
public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IStringLocalizer<UserService> _localizer;
    private readonly ApplicationDbContext _context;

    private readonly ICurrentUser _currentUser;
    private readonly MailSettings _mailSettings;
    private readonly MepdTenantInfo _currentTenant;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IStringLocalizer<UserService> localizer,
        ApplicationDbContext context,
        ICurrentUser currentUser,
        IOptions<MailSettings> mailSettings,
        MepdTenantInfo currentTenant,
        IEmailTemplateService emailTemplateService,
        IJobService jobService, IMailService mailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _localizer = localizer;
        _context = context;
        _currentUser = currentUser;
        _mailSettings = mailSettings.Value;
        _currentTenant = currentTenant;
        _emailTemplateService = emailTemplateService;
        _jobService = jobService;
        _mailService = mailService;
    }

    /// <inheritdoc />
    public async Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter)
    {
        var spec = new EntitiesByPaginationFilterSpec<ApplicationUser>(filter);

        var users = await _userManager.Users
            .WithSpecification(spec)
            .ProjectToType<UserDetailsDto>()
            .ToListAsync();

        int count = await _userManager.Users
            .CountAsync();

        return new PaginationResponse<UserDetailsDto>(users, count, filter.PageNumber, filter.PageSize);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsWithNameAsync(string username, string excludeId = null)
    {
        return await _userManager.FindByNameAsync(username) is ApplicationUser user && user.Id != excludeId;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string excludeId = null)
    {
        return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is ApplicationUser user && user.Id != excludeId;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsWithEmailAsync(string email, string excludeId = null)
    {
        return await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user && user.Id != excludeId;
    }

    /// <inheritdoc />
    public async Task<List<UserDetailsDto>> GetAllAsync()
    {
        return (await _userManager.Users
                .AsNoTracking()
                .ToListAsync())
                .Adapt<List<UserDetailsDto>>();
    }

    /// <inheritdoc />
    public async Task<UserDetailsDto> GetAsync(string userId)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        return user.Adapt<UserDetailsDto>();
    }

    /// <inheritdoc />
    public async Task<IList<UserRoleDto>> GetRolesAsync(string userId)
    {
        var userRoles = new List<UserRoleDto>();

        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();
        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                Enabled = await _userManager.IsInRoleAsync(user, role.Name)
            });
        }

        return userRoles;
    }

    /// <inheritdoc />
    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        // Criteria : Check if Current Tenant has atleast 2 Admins and current user is not Root Tenant Admin
        // Check is there is a Disable flag for Admin in the request
        var disabledAdminRoleInRequest = request.UserRoles.Find(a => !a.Enabled && a.RoleName == MepdRoles.Admin);


        if (disabledAdminRoleInRequest is not null)
        {
            var adminRole = await _roleManager.Roles.Where(r => r.Name == MepdRoles.Admin).FirstOrDefaultAsync();

            if (adminRole is not null)
            {
                // Guarantees Admin Role is available and the request has Disable flag for Admin Role
                // Now get count of Users with Admin Role
                int adminCount = await _context.UserRoles.Where(a => a.RoleId == adminRole.Id).CountAsync();

                // Now get count of Users with Admin Role
                bool hasRootEmailAddress = user.Email == MultitenancyConstants.Root.EmailAddress;

                if (hasRootEmailAddress)
                {
                    string? tenantOfUser = await _context.Users.Where(a => a.Id == userId).Select(x => EF.Property<string>(x, "TenantId")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(tenantOfUser) && tenantOfUser == MultitenancyConstants.Root.Id)
                    {
                        throw new ConflictException(_localizer["Cannot Remove Admin Role From Root Tenant Admin."]);
                    }
                }
                else if (adminCount <= 2)
                {
                    throw new ConflictException(_localizer["Tenant should have atleast 2 Admins."]);
                }
            }
        }

        foreach (var userRole in request.UserRoles)
        {
            // Check if Role Exists
            if (await _roleManager.FindByNameAsync(userRole.RoleName) is not null)
            {
                if (userRole.Enabled)
                {
                    if (!await _userManager.IsInRoleAsync(user, userRole.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, userRole.RoleName);
                    }
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole.RoleName);
                }
            }
        }

        return _localizer["User Roles Updated Successfully."];
    }

    /// <inheritdoc/>
    public async Task<bool> CheckPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
        if (string.IsNullOrEmpty(permission)) throw new ArgumentNullException(nameof(permission));

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(string.Format(_localizer["User '{0}' not found!"]));
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var roleName in userRoles)
        {
            foreach (var role in _roleManager.Roles.Where(e => e.Name.Contains(roleName)).ToList())
            {
                if (await _context.RoleClaims.Where(e => e.RoleId == role.Id && e.ClaimType == MepdClaims.Permission && e.ClaimValue == permission).AnyAsync(cancellationToken: cancellationToken))
                {
                    return true;
                }
            }

        }

        return false;
    }

    /// <inheritdoc />
    public async Task<List<PermissionDto>> GetPermissionsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var permissions = new List<PermissionDto>();
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in await _roleManager.Roles
            .Where(r => userRoles.Contains(r.Name))
            .ToListAsync())
        {
            var roleClaims = await _context.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == MepdClaims.Permission)
                .ToListAsync();
            permissions.AddRange(roleClaims.Adapt<List<PermissionDto>>());
        }

        return permissions.Distinct().ToList();
    }

    /// <inheritdoc />
    public async Task<string> DeleteAsync(string userId)
    {
        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        if (userId == _currentUser.GetUserId())
        {
            throw new ConflictException(_localizer["users.cannotbedeleted."]);
        }

        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded
            ? string.Format(_localizer["User {0} Delete."], userId)
            : throw new InternalServerException(_localizer["Delete user failed"], result.Errors.Select(e => _localizer[e.Description].ToString()).ToList());
    }

    /// <inheritdoc />
    public async Task<UserDetailsDto> CreateUserAsync(UserCreateRequest request, string origin)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Username,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.IsActive,
            PhoneNumberConfirmed = request.IsPhoneNumberConfirmed,
            EmailConfirmed = request.IsEmailConfirmed
        };

        var password = GeneratePassword();

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new InternalServerException(_localizer["Validation Errors Occurred."], result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }

        await _userManager.AddToRoleAsync(user, MepdRoles.Basic);

        var messages = new List<string> { string.Format(_localizer["User {0} Created."], user.UserName) };

        if (_mailSettings.EnableVerification && !string.IsNullOrEmpty(user.Email))
        {
            string verificationUrl = await GetEmailVerificationUriAsync(user, origin);
            var args = new Dictionary<string, string>
            {
                { "Email", user.Email },
                { "Password", password },
                { "UserName", user.UserName ?? "User" },
                { "EmailVerificationUri", verificationUrl }
            };
            var mailRequest = new MailRequest(
                new List<string> { user.Email },
                _localizer["Confirm Account"],
                _emailTemplateService.GenerateEmailFromTemlateMail(args, "email-newaccount"));

            _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
            messages.Add(_localizer[$"A verficiation email sent to {user.Email}!"]);
        }

        var createdAccount = user.Adapt<UserDetailsDto>();
        return createdAccount;
    }

    /// <inheritdoc />
    public async Task<UserDetailsDto> UpdateUserAsync(string userId, UserUpdateRequest request, string origin)
    {
        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var emailChanged = false;

        if (!string.IsNullOrEmpty(request.FirstName) && !user.FirstName.Equals(request.FirstName, StringComparison.OrdinalIgnoreCase))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName) && !user.LastName.Equals(request.LastName, StringComparison.OrdinalIgnoreCase))
            user.LastName = request.LastName;

        if (!string.Equals(user.PhoneNumber, request.PhoneNumber, StringComparison.OrdinalIgnoreCase))
        {
            user.PhoneNumber = request.PhoneNumber;
        }
        if (request.IsPhoneNumberConfirmed is not null && user.PhoneNumberConfirmed != request.IsPhoneNumberConfirmed)
        {
            user.PhoneNumberConfirmed = request.IsPhoneNumberConfirmed.Value;
        }

        if (!string.IsNullOrEmpty(request.Email) && !user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = request.Email;
            emailChanged = true;
        }

        if (request.IsEmailConfirmed is not null && user.EmailConfirmed != request.IsEmailConfirmed)
        {
            user.EmailConfirmed = request.IsEmailConfirmed.Value;
        }

        if (request.IsActive is not null && user.IsActive != request.IsActive)
        {
            user.IsActive = request.IsActive.Value;
        }

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new InternalServerException(_localizer["Validation Errors Occurred"], result.Errors.Select(e => _localizer[e.Description].ToString()).ToList());
        }

        var messages = new List<string> { string.Format(_localizer["User {0} update."], user.UserName) };

        if (_mailSettings.EnableVerification && emailChanged && !user.EmailConfirmed)
        {
            string verificationUrl = await GetEmailVerificationUriAsync(user, origin);
            var args = new Dictionary<string, string>
            {
                { "Email", user.Email },
                { "UserName", user.UserName ?? "User" },
                { "EmailVerificationUri", verificationUrl }
            };
            var mailRequest = new MailRequest(
                new List<string> { user.Email },
                _localizer["Confirm Account"],
                _emailTemplateService.GenerateEmailFromTemlateMail(args, "email-confirmation"));

            _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
            messages.Add(_localizer[$"A verficiation email sent to {user.Email}!"]);
        }

        var updatedAccount = user.Adapt<UserDetailsDto>();
        return updatedAccount;
    }

    /// <inheritdoc />
    public async Task<int> GetCountAsync()
    {
        return await _userManager.Users.AsNoTracking().CountAsync();
    }

    /// <inheritdoc />
    public async Task ToggleUserStatusAsync(ToggleUserStatusRequest request)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync();

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        bool isAdmin = await _userManager.IsInRoleAsync(user, MepdRoles.Admin);
        if (isAdmin)
        {
            throw new ConflictException(_localizer["Administrators Profile's Status cannot be toggled"]);
        }

        if (user != null)
        {
            user.IsActive = request.ActivateUser;
            var identityResult = await _userManager.UpdateAsync(user);
        }
    }

    private string GeneratePassword()
    {
        var options = _userManager.Options.Password;
        var length = options.RequiredLength;
        var nonAlphanumeric = options.RequireNonAlphanumeric;
        var digit = options.RequireDigit;
        var lowercase = options.RequireLowercase;
        var uppercase = options.RequireUppercase;

        var password = new StringBuilder();
        var random = new Random();

        while (password.Length < length)
        {
            char c = (char)random.Next(32, 126);

            password.Append(c);

            if (char.IsDigit(c))
                digit = false;
            else if (char.IsLower(c))
                lowercase = false;
            else if (char.IsUpper(c))
                uppercase = false;
            else if (!char.IsLetterOrDigit(c))
                nonAlphanumeric = false;
        }

        if (nonAlphanumeric)
            password.Append((char)random.Next(33, 48));
        if (digit)
            password.Append((char)random.Next(48, 58));
        if (lowercase)
            password.Append((char)random.Next(97, 123));
        if (uppercase)
            password.Append((char)random.Next(65, 91));

        return password.ToString();
    }

    private async Task<string> GetEmailVerificationUriAsync(ApplicationUser user, string origin)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        const string route = "confirm-email";
        var endpointUri = new Uri($"{origin}/{route}");
        var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), QueryStringKeys.UserId, user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, QueryStringKeys.Code, code);
        if (_currentTenant.Id is string tenantKey)
            verificationUri = QueryHelpers.AddQueryString(verificationUri, MultitenancyConstants.TenantIdName, tenantKey);
        return verificationUri;
    }
}