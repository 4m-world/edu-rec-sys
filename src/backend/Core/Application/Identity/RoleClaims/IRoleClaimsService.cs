namespace CodeMatrix.Mepd.Application.Identity.RoleClaims;

public interface IRoleClaimsService : ITransientService
{
    public Task<bool> HasPermissionAsync(string userId, string permission);

    Task<List<RoleClaimDto>> GetAllAsync();

    Task<int> GetCountAsync();

    Task<RoleClaimDto> GetByIdAsync(int id);

    Task<List<RoleClaimDto>> GetAllByRoleIdAsync(string roleId);

    Task<string> SaveAsync(RoleClaimRequest request);

    Task<string> DeleteAsync(int id);
}