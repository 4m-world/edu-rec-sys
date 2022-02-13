using CodeMatrix.Mepd.Application.Common.Exceptions;
using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Application.Common.Models;
using CodeMatrix.Mepd.Application.Identity.Roles;

namespace CodeMatrix.Mepd.Application.Identity.Users;

/// <summary>
/// User service contract
/// </summary>
public interface IUserService : ITransientService
{
    /// <summary>
    /// Search users 
    /// </summary>
    /// <param name="filter">User list filter</param>
    /// <returns>Pagianted list of user detailas</returns>
    Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter);

    /// <summary>
    /// Check if username already exists
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="excludeId">Identifier to exclude</param>
    /// <returns>True if exisits, otherwise false</returns>
    Task<bool> ExistsWithNameAsync(string username, string excludeId = null);

    /// <summary>
    /// Check if phone number is asscoicated with another account
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="excludeId"></param>
    /// <returns></returns>
    Task<bool> ExistsWithPhoneNumberAsync(string phone, string excludeId = null);

    /// <summary>
    /// Check if email address already exists
    /// </summary>
    /// <param name="emailAddress">Email address</param>
    /// <param name="excludeId">Identifier to exclude</param>
    /// <returns>True if exisits, otherwise false</returns>
    Task<bool> ExistsWithEmailAsync(string emailAddress, string excludeId = null);


    /// <summary>
    /// Get all users 
    /// </summary>
    /// <returns>List ff user details</returns>
    Task<List<UserDetailsDto>> GetAllAsync();

    /// <summary>
    /// Get number of users
    /// </summary>
    /// <returns>number of records</returns>
    Task<int> GetCountAsync();

    /// <summary>
    /// Ge user 
    /// </summary>
    /// <param name="userId">user identifier</param>
    /// <returns>User details</returns>
    Task<UserDetailsDto> GetAsync(string userId);

    /// <summary>
    /// Get user roles
    /// </summary>
    /// <param name="userId">user identifer</param>
    /// <returns>User role</returns>
    Task<IList<UserRoleDto>> GetRolesAsync(string userId);

    /// <summary>
    /// Assign user to role
    /// </summary>
    /// <param name="userId">user identifier</param>
    /// <param name="request">user role request</param>
    /// <returns>assigned user idnetifer</returns>
    Task<string> AssignRolesAsync(string userId, UserRolesRequest request);

    /// <summary>
    /// Get user assigned permissions
    /// </summary>
    /// <param name="id">user idnetifier</param>
    /// <returns>List of permissions</returns>
    Task<List<PermissionDto>> GetPermissionsAsync(string id);

    /// <summary>
    /// Deletes user
    /// </summary>
    /// <param name="userId">user identifier</param>
    /// <returns>Delete user identifier</returns>
    Task<string> DeleteAsync(string userId);


    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">User create request</param>
    /// <param name="origin">Request origin</param>
    /// <returns>Created user details</returns>
    Task<UserDetailsDto> CreateUserAsync(UserCreateRequest request, string origin);

    /// <summary>
    /// Updates user data
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="request">User update request</param>
    /// <param name="origin">Request origin</param>
    /// <returns>Updated user details</returns>
    Task<UserDetailsDto> UpdateUserAsync(string userId, UserUpdateRequest request, string origin);

    /// <summary>
    /// Toggle user status
    /// </summary>
    /// <param name="request">Toggle user status request</param>
    Task ToggleUserStatusAsync(ToggleUserStatusRequest request);

    /// <summary>
    /// Check if user account is assigned with a permission
    /// </summary>
    /// <param name="userId">User account identifier</param>
    /// <param name="permission">Permission To check</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="permission"/>
    /// <paramref name="permission"/>
    /// </exception>
    /// <exception cref="NotFoundException">
    /// <paramref name="userId"/>
    /// </exception>
    Task<bool> CheckPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
}