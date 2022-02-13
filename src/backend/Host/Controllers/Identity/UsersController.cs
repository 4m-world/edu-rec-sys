using CodeMatrix.Mepd.Application.Identity.Roles;
using CodeMatrix.Mepd.Application.Identity.Users;
using CodeMatrix.Mepd.Application.Wrapper;
using CodeMatrix.Mepd.Infrastructure.Middleware;

namespace CodeMatrix.Mepd.Host.Controllers.Identity;

/// <summary>
/// Users controller
/// </summary>
public class UsersController : VersionNeutralApiController
{
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentuser;

    private string OriginFromRequest => Request.Headers.Origin.FirstOrDefault() ?? $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";

    /// <summary>
    /// intizliae user controller
    /// </summary>
    /// <param name="userService">User service</param>
    /// <param name="currentuser">Current user service</param>
    public UsersController(IUserService userService, ICurrentUser currentuser)
    {
        _userService = userService;
        _currentuser = currentuser;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <param name="filter">Users filter</param>
    [HttpGet]
    [MustHavePermission(MepdPermissions.Users.ViewAll)]
    public async Task<ActionResult<PaginationResponse<UserDetailsDto>>> GetAllAsync([FromQuery] UsersFilter filter)
    {
        var users = await _userService.SearchAsync(new UserListFilter
        {
            IsActive = filter.IsActive,
            Keyword = filter.Search,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            OrderBy = filter.OrderBy,
        });
        return Ok(users);
    }

    /// <summary>
    /// Search users
    /// </summary>
    /// <param name="search">Search keywords</param>
    /// <param name="filter">User filter</param>
    [HttpGet("search")]
    [MustHavePermission(MepdPermissions.Users.Search)]
    public async Task<ActionResult<PaginationResponse<UserDetailsDto>>> SearchUsers(string search, [FromQuery] BasicPaginationFilter filter)
    {
        var users = await _userService.SearchAsync(new UserListFilter
        {
            IsActive = true,
            Keyword = search,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            OrderBy = filter.OrderBy,
        });
        return Ok(users);
    }

    /// <summary>
    /// Create new user
    /// </summary>
    /// <param name="request">Create user request</param>
    [HttpPost]
    [MustHavePermission(MepdPermissions.Users.Create)]
    [TenantIdHeader]
    [OpenApiOperation("Create new user account")]
    [ProducesDefaultResponseType(typeof(ErrorResult))]
    public async Task<ActionResult<Result<UserDetailsDto>>> CreateAsync([FromBody] UserCreateRequest request)
    {
        var result = await _userService.CreateUserAsync(request, OriginFromRequest);
        return Ok(Result<UserDetailsDto>.Success(result));
    }

    /// <summary>
    /// Update user 
    /// </summary>
    /// <param name="id">User identifier</param>
    /// <param name="request">User update request</param>
    [HttpPut("{id}")]
    [MustHavePermission(MepdPermissions.Users.Edit)]
    [TenantIdHeader]
    [OpenApiOperation("Update user account main details")]
    [ProducesDefaultResponseType(typeof(ErrorResult))]
    public async Task<ActionResult<UserDetailsDto>> UpdateAsync(string id, [FromBody] UserUpdateRequest request)
    {
        request.Id = id;
        var result = await _userService.UpdateUserAsync(id, request, OriginFromRequest);
        return Ok(Result<UserDetailsDto>.Success(result));
    }

    /// <summary>
    /// Get user details
    /// </summary>
    /// <param name="id">User identifer</param>
    [HttpGet("{id}")]
    [MustHavePermission(MepdPermissions.Users.View)]
    public async Task<ActionResult<Result<UserDetailsDto>>> GetByIdAsync(string id)
    {
        var user = await _userService.GetAsync(id);
        return Ok(Result<UserDetailsDto>.Success(user));
    }

    /// <summary>
    /// Get user assigned tles
    /// </summary>
    /// <param name="id">user identifer</param>
    /// <returns></returns>
    [HttpGet("{id}/roles")]
    [MustHavePermission(MepdPermissions.Users.View)]
    public async Task<ActionResult<Result<IList<UserRoleDto>>>> GetRolesAsync(string id)
    {
        var userRoles = await _userService.GetRolesAsync(id);
        return Ok(Result<IList<UserRoleDto>>.Success(userRoles));
    }

    /// <summary>
    /// Get user permission
    /// </summary>
    /// <param name="id">User identifier</param>
    [HttpGet("{id}/permissions")]
    [MustHavePermission(MepdPermissions.Users.View)]
    public async Task<ActionResult<Result<List<PermissionDto>>>> GetPermissionsAsync(string id)
    {
        var userPermissions = await _userService.GetPermissionsAsync(id);
        return Ok(Result<IList<PermissionDto>>.Success(userPermissions));
    }

    /// <summary>
    /// Assigned user to a role
    /// </summary>
    /// <param name="id">User identifer</param>
    /// <param name="request">User role request</param>
    [HttpPost("{id}/roles")]
    [ProducesResponseType(200)]
    [ProducesDefaultResponseType(typeof(ErrorResult))]
    [MustHavePermission(MepdPermissions.Users.Edit)]
    public async Task<ActionResult<Result<string>>> AssignRolesAsync(string id, UserRolesRequest request)
    {
        var result = await _userService.AssignRolesAsync(id, request);
        return Ok(Result<string>.Success(result));
    }

    /// <summary>
    /// Get current user assigned permissions
    /// </summary>
    [HttpGet("my/permissions")]
    [ProducesResponseType(200)]
    [OpenApiOperation("Get list of permissions granted to the logged user")]
    [MustHavePermission(MepdPermissions.Users.ViewOwn)]
    public async Task<ActionResult<Result<List<PermissionDto>>>> GetCurrentUserPermissionsAsync()
    {
        var userId = _currentuser.GetUserId();
        var userPermissions = await _userService.GetPermissionsAsync(userId);
        return Ok(Result<IList<PermissionDto>>.Success(userPermissions));
    }

    /// <summary>
    /// Deletes user accout
    /// </summary>
    /// <param name="id">user identifier</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesDefaultResponseType(typeof(ErrorResult))]
    [OpenApiOperation("Delete user account")]
    [MustHavePermission(MepdPermissions.Users.Delete)]
    public async Task<ActionResult<Result<string>>> DeleteUserAsync(string id)
    {
        return Ok(Result<string>.Success(await _userService.DeleteAsync(id)));
    }

    /// <summary>
    /// Toggle user satus
    /// </summary>
    /// <param name="request">Toggle user stauts request</param>
    [HttpPost("toggle-status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(HttpValidationProblemDetails))]
    [ProducesDefaultResponseType(typeof(ErrorResult))]
    public Task ToggleUserStatusAsync(ToggleUserStatusRequest request)
    {
        return _userService.ToggleUserStatusAsync(request);
    }

    private string GenerateOrigin()
    {
        return $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
    }
}