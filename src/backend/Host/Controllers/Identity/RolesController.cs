using CodeMatrix.Mepd.Application.Common.Models;
using CodeMatrix.Mepd.Application.Identity.Roles;
using CodeMatrix.Mepd.Application.Identity.Users;
using CodeMatrix.Mepd.Application.Wrapper;
using CodeMatrix.Mepd.Infrastructure.Auth.Permissions;
using CodeMatrix.Mepd.Shared.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMatrix.Mepd.Host.Controllers.Identity;

/// <summary>
/// Roles controller
/// </summary>
public class RolesController : VersionNeutralApiController
{
    private readonly IRoleService _roleService;

    /// <summary>
    /// Const.
    /// </summary>
    /// <param name="roleService">Role service</param>
    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Get roles
    /// </summary>
    [HttpGet("all")]
    [MustHavePermission(MepdPermissions.Roles.ViewAll)]
    public async Task<PaginationResponse<RoleDto>> GetListAsync(string search, int pageNumber = 1, int pageSize = 10, string orderBy = null)
    {
        var roles = await _roleService.GetListAsync(pageNumber, pageSize, search, !string.IsNullOrWhiteSpace(orderBy) ? orderBy.Split(",") : null);
        return roles;
    }

    /// <summary>
    /// Get role by id
    /// </summary>
    [HttpGet("{id}")]
    [MustHavePermission(MepdPermissions.Roles.View)]
    public async Task<ActionResult<Result<RoleDto>>> GetByIdAsync(string id)
    {
        var roles = await _roleService.GetByIdAsync(id);
        return Ok(Result<RoleDto>.Success(roles));
    }

    /// <summary>
    /// Get role permissions
    /// </summary>
    [HttpGet("{id}/permissions")]
    [MustHavePermission(MepdPermissions.Roles.View)]
    public async Task<ActionResult<Result<List<PermissionGroup>>>> GetPermissionsAsync(string id)
    {
        var roles = await _roleService.GetPermissionsAsync(id);
        return Ok(Result<List<PermissionGroup>>.Success(roles));
    }

    /// <summary>
    /// Update role permission status
    /// </summary>
    [HttpPut("{id}/permissions")]
    [MustHavePermission(MepdPermissions.RoleClaims.Edit)]
    public async Task<ActionResult<Result<string>>> UpdatePermissionsAsync(string id, List<UpdatePermissionsRequest> request)
    {
        var roles = await _roleService.UpdatePermissionsAsync(id, request);
        return Ok(Result<string>.Success(roles));
    }

    /// <summary>
    /// Create new role
    /// </summary>
    [HttpPost]
    [MustHavePermission(MepdPermissions.Roles.Register)]
    public async Task<ActionResult<Result<string>>> RegisterRoleAsync(RoleCreateRequest request)
    {
        var response = await _roleService.RegisterRoleAsync(request);
        return Ok(Result<string>.Success(response));
    }

    /// <summary>
    /// Update a role
    /// </summary>
    [HttpPut]
    [MustHavePermission(MepdPermissions.Roles.Update)]
    public async Task<string> UpdateRoleAsync(RoleUpdateRequest request)
    {
        var response = await _roleService.UpdateRoleAsync(request);
        return response;
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="id">Role id</param>
    [HttpDelete("{id}")]
    [MustHavePermission(MepdPermissions.Roles.Remove)]
    public async Task<ActionResult<Result<string>>> DeleteAsync(string id)
    {
        var response = await _roleService.DeleteAsync(id);
        return Ok(Result<string>.Success(response));
    }

    /// <summary>
    /// Get users under the role
    /// </summary>
    /// <param name="roleId">Role id</param>
    /// <param name="filter">User filter</param>
    [HttpGet("{roleId}/users")]
    [MustHavePermission(MepdPermissions.Roles.View)]
    public async Task<ActionResult<PaginationResponse<UserDetailsDto>>> GetRoleUsers(string roleId, [FromQuery] UsersFilter filter)
    {
        var response = await _roleService.GetRoleUsers(roleId, filter);
        return Ok(response);
    }
}