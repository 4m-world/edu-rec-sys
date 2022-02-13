using CodeMatrix.Mepd.Application.Common.Exceptions;
using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Application.Common.Models;
using CodeMatrix.Mepd.Application.Common.Persistence;
using CodeMatrix.Mepd.Application.Identity.RoleClaims;
using CodeMatrix.Mepd.Application.Identity.Roles;
using CodeMatrix.Mepd.Application.Identity.Users;
using CodeMatrix.Mepd.Infrastructure.Common.Extensions;
using CodeMatrix.Mepd.Infrastructure.Identity.Extensions;
using CodeMatrix.Mepd.Infrastructure.Mapping;
using CodeMatrix.Mepd.Infrastructure.Persistence;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using CodeMatrix.Mepd.Shared.Authorization;
using CodeMatrix.Mepd.Shared.Multitenancy;
using Finbuckle.MultiTenant;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace CodeMatrix.Mepd.Infrastructure.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer<RoleService> _localizer;
    private readonly ICurrentUser _currentUser;
    private readonly IRoleClaimsService _roleClaimService;
    private readonly IDapperRepository _repository;
    private readonly ITenantInfo _tenantInfo;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IStringLocalizer<RoleService> localizer,
        ICurrentUser currentUser,
        IRoleClaimsService roleClaimService,
        IDapperRepository repository,
        ITenantInfo tenantInfo)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _localizer = localizer;
        _currentUser = currentUser;
        _roleClaimService = roleClaimService;
        _repository = repository;
        _tenantInfo = tenantInfo;
    }

    public async Task<string> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(id);

        _ = role ?? throw new NotFoundException(_localizer["Role Not Found"]);

        if (DefaultRoles.Contains(role.Name))
        {
            throw new ConflictException(string.Format(_localizer["Not allowed to delete {0} Role."], role.Name));
        }

        bool roleIsNotUsed = true;
        var allUsers = await _userManager.Users.ToListAsync(cancellationToken: cancellationToken);
        foreach (var user in allUsers)
        {
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                roleIsNotUsed = false;
            }
        }

        if (roleIsNotUsed)
        {
            await _roleManager.DeleteAsync(role);
            return string.Format(_localizer["Role {0} Deleted."], role.Name);
        }
        else
        {
            throw new ConflictException(string.Format(_localizer["Not allowed to delete {0} Role as it is being used."], role.Name));
        }
    }

    public async Task<RoleDto> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);

        _ = role ?? throw new NotFoundException(_localizer["Role Not Found"]);

        var roleDto = role.Adapt<RoleDto>();
        roleDto.IsDefault = DefaultRoles.Contains(role.Name);

        return roleDto;
    }

    public async Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken = default)
    {
        var role = await GetByIdAsync(roleId, cancellationToken);

        role.Permissions = await GetPermissionsAsync(roleId, cancellationToken);

        role.IsRootRole = _tenantInfo.Id == MultitenancyConstants.Root.Id;

        return role;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _roleManager.Roles.CountAsync(cancellationToken: cancellationToken);
    }

    public async Task<PaginationResponse<RoleDto>> GetListAsync(int pageIndex, int pageSize, string search = default, string[] orderBy = default, CancellationToken cancellationToken = default)
    {
        var query = _roleManager.Roles;

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.SearchByKeyword(search);
        }

        if (orderBy?.Any() ?? false)
        {
            query = query.OrderBy(string.Join(",", orderBy));
        }
        else { query = query.OrderBy(e => e.Name); }

        var rolesResponse = await query.ToMappedPaginatedResultAsync<ApplicationRole, RoleDto>(pageIndex, pageSize, cancellationToken: cancellationToken);
        foreach (var role in rolesResponse.Data)
        {
            role.IsDefault = DefaultRoles.Contains(role.Name);
        }

        return rolesResponse;
    }

    public async Task<List<PermissionGroup>> GetPermissionsAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleExits = await _context.Roles.AnyAsync(e => e.Id == id, cancellationToken: cancellationToken);
        if (!roleExits)
        {
            throw new NotFoundException(_localizer["roles.roleNotFound"]);
        }

        var rolePermissions = await _context.RoleClaims.Where(a => a.RoleId == id && a.ClaimType == MepdClaims.Permission).ToListAsync(cancellationToken: cancellationToken);

        var groups = typeof(MepdPermissions).GetNestedTypes();
        var permissionGroups = new List<PermissionGroup>();

        //var list = new List<PermissionDto>();

        foreach (var group in groups)
        {
            var permissionGroup = new PermissionGroup
            {
                Name = group.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? _localizer[$"permission.group{group.Name}"],
                Description = group.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description
            };

            var fields = group.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var permissions = new List<PermissionDto>();

            foreach (var field in fields)
            {
                var propertyValue = field.GetValue(null);

                if (propertyValue is null) continue;

                var permission = new PermissionDto
                {
                    Permission = field.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Name ?? _localizer[$"permission.{field.Name}"],
                    Description = field.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description,
                    Key = propertyValue.ToString()
                };
                permission.IsGranted = rolePermissions.Any(p => p.ClaimValue == permission.Key);

                permissions.Add(permission);
            }
            permissionGroup.Permissions = permissions;

            permissionGroups.Add(permissionGroup);
        }

        return permissionGroups;
    }

    public async Task<List<RoleDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _context.UserRoles.Where(a => a.UserId == userId).Select(a => a.RoleId).ToListAsync(cancellationToken: cancellationToken);
        var roles = await _roleManager.Roles.Where(a => userRoles.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken);

        var roleDtos = roles.Adapt<List<RoleDto>>();
        roleDtos.ForEach(role => role.IsDefault = DefaultRoles.Contains(role.Name));

        return roleDtos;
    }

    public async Task<string> RegisterRoleAsync(RoleCreateRequest request, CancellationToken cancellationToken = default)
    {
        var newRole = new ApplicationRole(request.Name, request.Description);
        var result = await _roleManager.CreateAsync(newRole);

        return result.Succeeded
            ? newRole.Id
            : throw new InternalServerException(_localizer["Register role failed"], result.Errors.Select(e => _localizer[e.Description].ToString()).ToList());
    }

    public async Task<string> UpdateRoleAsync(RoleUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);

        _ = role ?? throw new NotFoundException(_localizer["Role Not Found"]);

        if (DefaultRoles.Contains(role.Name))
        {
            throw new ConflictException(string.Format(_localizer["Not allowed to modify {0} Role."], role.Name));
        }

        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpperInvariant();
        role.Description = request.Description;
        var result = await _roleManager.UpdateAsync(role);

        return result.Succeeded
            ? string.Format(_localizer["Role {0} Updated."], role.Name)
            : throw new InternalServerException(_localizer["Update role failed"], result.Errors.Select(e => _localizer[e.Description].ToString()).ToList());
    }

    public async Task<bool> ExistsAsync(string roleName, string excludeId, CancellationToken cancellationToken = default) =>
        await _roleManager.FindByNameAsync(roleName)
            is ApplicationRole existingRole
            && existingRole.Id != excludeId;

    public async Task<string> UpdatePermissionsAsync(string roleId, List<UpdatePermissionsRequest> request, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        var role = await _roleManager.FindByIdAsync(roleId);
        _ = role ?? throw new NotFoundException(_localizer["Role Not Found"]);

        if (role.Name == MepdRoles.Admin)
        {
            var currentUser = await _userManager.Users.SingleAsync(x => x.Id == _currentUser.GetUserId().ToString(), cancellationToken: cancellationToken);
            if (await _userManager.IsInRoleAsync(currentUser, MepdRoles.Admin))
            {
                throw new ConflictException(_localizer["Not allowed to modify Permissions for this Role."]);
            }
        }

        if (_tenantInfo.Id != MultitenancyConstants.Root.Id)
        {
            request.RemoveAll(u => u.Permission.StartsWith("Permissions.Root."));
        }

        var selectedPermissions = request.Where(a => a.Enabled).ToList();
        if (role.Name == MepdRoles.Admin)
        {
            if (!selectedPermissions.Any(x => x.Permission == MepdPermissions.Roles.View)
               || !selectedPermissions.Any(x => x.Permission == MepdPermissions.RoleClaims.View)
               || !selectedPermissions.Any(x => x.Permission == MepdPermissions.RoleClaims.Edit))
            {
                throw new ConflictException(string.Format(
                    _localizer["Not allowed to deselect {0} or {1} or {2} for this Role."],
                    MepdPermissions.Roles.View,
                    MepdPermissions.RoleClaims.View,
                    MepdPermissions.RoleClaims.Edit));
            }
        }

        var permissions = await _roleManager.GetClaimsAsync(role);
        foreach (var claim in permissions.Where(p => request.Any(a => a.Permission == p.Value)))
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }

        foreach (var permission in selectedPermissions)
        {
            if (!string.IsNullOrEmpty(permission.Permission))
            {
                var addResult = await _roleManager.AddPermissionClaimAsync(role, permission.Permission);
                if (!addResult.Succeeded)
                {
                    errors.AddRange(addResult.Errors.Select(e => _localizer[e.Description].ToString()));
                }
            }
        }

        var allPermissions = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
        foreach (var permission in selectedPermissions)
        {
            if (allPermissions.SingleOrDefault(x => x.Type == MepdClaims.Permission && x.Value == permission.Permission)
                is RoleClaimDto addedPermission)
            {
                await _roleClaimService.SaveAsync(addedPermission.Adapt<RoleClaimRequest>());
            }
        }

        if (errors.Count > 0)
        {
            throw new InternalServerException(_localizer["Update permissions failed."], errors);
        }

        return _localizer["Permissions Updated."];
    }

    private const string USERS_UNDER_ROLE = @"
SELECT  U.Id, U.UserName, U.FirstName, U.LastName, U.Email,
        U.IsActive
  FROM [IDENTITY].[Users] AS U INNER JOIN [IDENTITY].[UserRoles] AS UR ON U.Id = UR.UserId
 WHERE UR.RoleId = @Id
   AND (@Search IS NULL OR 
        (UserName LIKE '%' + @Search + '%') OR 
        (FirstName LIKE '%' + @Search + '%') OR 
        (LastName LIKE '%' + @Search + '%') OR
        (Email LIKE '%' + @Search + '%')
       )
 ORDER BY U.UserName";

    public async Task<PaginationResponse<UserDetailsDto>> GetRoleUsers(string roleId, UsersFilter filter, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            throw new NotFoundException(_localizer["Role does not exist."]);
        }
        return await _repository.QueryAsync<UserDetailsDto>(USERS_UNDER_ROLE, filter.PageNumber, filter.PageSize, new { Id = roleId, Search = filter.Search }, cancellationToken: cancellationToken);
    }
    internal static List<string> DefaultRoles =>
        typeof(MepdRoles).GetAllPublicConstantValues<string>();
}