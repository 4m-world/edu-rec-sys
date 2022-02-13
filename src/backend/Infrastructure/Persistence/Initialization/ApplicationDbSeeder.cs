using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using CodeMatrix.Mepd.Infrastructure.Multitenancy;
using CodeMatrix.Mepd.Infrastructure.Identity;
using CodeMatrix.Mepd.Infrastructure.Auth.Permissions;
using CodeMatrix.Mepd.Shared.Multitenancy;
using CodeMatrix.Mepd.Shared.Authorization;
using CodeMatrix.Mepd.Infrastructure.Common.Extensions;

namespace CodeMatrix.Mepd.Infrastructure.Persistence.Initialization;

internal class ApplicationDbSeeder
{
    private readonly MepdTenantInfo _currentTenant;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly CustomSeederRunner _seederRunner;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(
        MepdTenantInfo currentTenant, 
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager, 
        CustomSeederRunner seederRunner, 
        ILogger<ApplicationDbSeeder> logger)
    {
        _currentTenant = currentTenant;
        _roleManager = roleManager;
        _userManager = userManager;
        _seederRunner = seederRunner;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
        await _seederRunner.RunSeedersAsync(cancellationToken);
    }

    private async Task SeedRolesAsync()
    {
        foreach (string roleName in RoleService.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                // Create the role
                role = new ApplicationRole(roleName, $"{roleName} Role for {_currentTenant.Id} Tenant");
                _logger.LogInformation("Seeding {role} Role for '{tenantId}' Tenant.", roleName, _currentTenant.Id);
                await _roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == MepdRoles.Basic)
            {
                await AssignPermissionsToRoleAsync(role, DefaultPermissions.Basics);
            }
            else if (roleName == MepdRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(role, typeof(MepdPermissions).GetNestedClassesStaticStringValues());

                if (_currentTenant.Id == MultitenancyConstants.Root.Id)
                {
                    await AssignPermissionsToRoleAsync(role, typeof(RootPermissions).GetNestedClassesStaticStringValues());
                }
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationRole role, List<string> permissions)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (string permission in permissions)
        {
            if (!currentClaims.Any(a => a.Type == MepdClaims.Permission && a.Value == permission))
            {
                _logger.LogInformation("Seeding {role} Permission '{permission}' for '{tenantId}' Tenant.", role.Name, permission, _currentTenant.Id);
                await _roleManager.AddClaimAsync(role, new Claim(MepdClaims.Permission, permission));
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        if (string.IsNullOrWhiteSpace(_currentTenant.Id) || string.IsNullOrWhiteSpace(_currentTenant.AdminEmail))
        {
            return;
        }

        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == _currentTenant.AdminEmail)
            is not ApplicationUser adminUser)
        {
            string adminUserName = $"{_currentTenant.Id.Trim()}.{MepdRoles.Admin}".ToLowerInvariant();
            adminUser = new ApplicationUser
            {
                FirstName = _currentTenant.Id.Trim().ToLowerInvariant(),
                LastName = MepdRoles.Admin,
                Email = _currentTenant.AdminEmail,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = _currentTenant.AdminEmail?.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                IsActive = true
            };

            _logger.LogInformation("Seeding Default Admin User for '{tenantId}' Tenant.", _currentTenant.Id);
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, MultitenancyConstants.DefaultPassword);
            await _userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, MepdRoles.Admin))
        {
            _logger.LogInformation("Assigning Admin Role to Admin User for '{tenantId}' Tenant.", _currentTenant.Id);
            await _userManager.AddToRoleAsync(adminUser, MepdRoles.Admin);
        }
    }
}
