using CodeMatrix.Mepd.Application.Identity.Users;

namespace CodeMatrix.Mepd.Application.Identity.Roles;

public interface IRoleService : ITransientService
{
    Task<PaginationResponse<RoleDto>> GetListAsync(int pageIndex, int pageSize, string search = default, string[] orderBy = default, CancellationToken cancellationToken = default);

    Task<List<PermissionGroup>> GetPermissionsAsync(string id, CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<RoleDto> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string roleName, string excludeId, CancellationToken cancellationToken = default);

    Task<string> RegisterRoleAsync(RoleCreateRequest request, CancellationToken cancellationToken = default);

    Task<string> DeleteAsync(string id, CancellationToken cancellationToken = default);

    Task<List<RoleDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);

    Task<string> UpdatePermissionsAsync(string id, List<UpdatePermissionsRequest> request, CancellationToken cancellationToken = default);

    Task<string> UpdateRoleAsync(RoleUpdateRequest request, CancellationToken cancellationToken = default);

    Task<PaginationResponse<UserDetailsDto>> GetRoleUsers(string roleId, UsersFilter filter, CancellationToken cancellationToken = default);
}