using CodeMatrix.Mepd.Application.Identity.RoleClaims;
using CodeMatrix.Mepd.Application.Wrapper;

namespace CodeMatrix.Mepd.Host.Controllers.Identity;

/// <summary>
/// Role cliam controller
/// </summary>
public class RoleClaimsController : VersionNeutralApiController
{
    private readonly IRoleClaimsService _roleClaimService;

    /// <summary>
    /// Const.
    /// </summary>
    /// <param name="roleClaimService">Role claim service</param>
    public RoleClaimsController(IRoleClaimsService roleClaimService)
    {
        _roleClaimService = roleClaimService;
    }

    /// <summary>
    /// Get all claims
    /// </summary>
    [MustHavePermission(MepdPermissions.RoleClaims.View)]
    [HttpGet]
    public async Task<ActionResult<Result<List<RoleClaimDto>>>> GetAllAsync()
    {
        var roleClaims = await _roleClaimService.GetAllAsync();
        return Ok(Result<IList<RoleClaimDto>>.Success(roleClaims));
    }

    /// <summary>
    /// Get all role claims
    /// </summary>
    /// <param name="roleId">Role id</param>
    [MustHavePermission(MepdPermissions.RoleClaims.View)]
    [HttpGet("{roleId}")]
    public async Task<ActionResult<Result<List<RoleClaimDto>>>> GetAllByRoleIdAsync([FromRoute] string roleId)
    {
        var response = await _roleClaimService.GetAllByRoleIdAsync(roleId);
        return Ok(Result<IList<RoleClaimDto>>.Success(response));
    }

    /// <summary>
    /// Add claim to the role
    /// </summary>
    [MustHavePermission(MepdPermissions.RoleClaims.Create)]
    [HttpPost]
    public async Task<ActionResult<Result<string>>> PostAsync(RoleClaimRequest request)
    {
        var response = await _roleClaimService.SaveAsync(request);
        return Ok(Result<string>.Success(response));
    }

    /// <summary>
    /// Delete a claim
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [MustHavePermission(MepdPermissions.RoleClaims.Delete)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Result<string>>> DeleteAsync(int id)
    {
        var response = await _roleClaimService.DeleteAsync(id);
        return Ok(Result<string>.Success(response));
    }
}