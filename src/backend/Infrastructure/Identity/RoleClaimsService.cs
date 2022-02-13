using CodeMatrix.Mepd.Application.Common.Caching;
using CodeMatrix.Mepd.Application.Common.Exceptions;
using CodeMatrix.Mepd.Application.Identity.RoleClaims;
using CodeMatrix.Mepd.Application.Identity.Roles;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using CodeMatrix.Mepd.Shared.Authorization;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CodeMatrix.Mepd.Infrastructure.Identity;

public class RoleClaimsService : IRoleClaimsService
{
    private readonly ApplicationDbContext _db;
    private readonly ICacheService _cache;
    private readonly IStringLocalizer<RoleClaimsService> _localizer;
    private readonly ICacheKeyService _cacheKey;

    public RoleClaimsService(
        ApplicationDbContext context,
        ICacheService cache,
        IStringLocalizer<RoleClaimsService> localizer,
        ICacheKeyService cacheKeyService)
    {
        _db = context;
        _cache = cache;
        _localizer = localizer;
        _cacheKey = cacheKeyService;
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission)
    {
        var roles = await _cache.GetOrSetAsync(
            _cacheKey.GetCacheKey(MepdClaims.Permission, userId),
            async () =>
            {
                var userRoles = await _db.UserRoles.Where(a => a.UserId == userId).Select(a => a.RoleId).ToListAsync();
                var applicationRoles = await _db.Roles.Where(a => userRoles.Contains(a.Id)).ToListAsync();
                return applicationRoles.Adapt<List<RoleDto>>();
            });

        if (roles is not null)
        {
            foreach (var role in roles)
            {
                if (_db.RoleClaims.Any(a => a.ClaimType == MepdClaims.Permission && a.ClaimValue == permission && a.RoleId == role.Id))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public async Task<List<RoleClaimDto>> GetAllAsync()
    {
        return (await _db.RoleClaims.ToListAsync())
            .Adapt<List<RoleClaimDto>>();
    }

    public Task<int> GetCountAsync() =>
        _db.RoleClaims.CountAsync();

    public async Task<RoleClaimDto> GetByIdAsync(int id)
    {
        var roleClaim = await _db.RoleClaims
            .SingleOrDefaultAsync(x => x.Id == id);

        _ = roleClaim ?? throw new NotFoundException(_localizer["RoleClaim Not Found"]);

        return roleClaim.Adapt<RoleClaimDto>();
    }

    public async Task<List<RoleClaimDto>> GetAllByRoleIdAsync(string roleId)
    {
        var roleClaims = await _db.RoleClaims
            .Where(x => x.RoleId == roleId)
            .ToListAsync();

        return roleClaims.Adapt<List<RoleClaimDto>>();
    }

    public async Task<string> SaveAsync(RoleClaimRequest request)
    {
        if (request.Id == 0)
        {
            var existingRoleClaim =
                await _db.RoleClaims
                    .SingleOrDefaultAsync(x =>
                        x.RoleId == request.RoleId && x.ClaimType == request.Type && x.ClaimValue == request.Value);
            if (existingRoleClaim is not null)
            {
                throw new ConflictException(_localizer["Similar Role Claim already exists."]);
            }

            var roleClaim = request.Adapt<ApplicationRoleClaim>();
            await _db.RoleClaims.AddAsync(roleClaim);
            await _db.SaveChangesAsync();

            return string.Format(_localizer["Role Claim {0} created."], request.Value);
        }
        else
        {
            var existingRoleClaim =
                await _db.RoleClaims
                    .SingleOrDefaultAsync(x => x.Id == request.Id);
            if (existingRoleClaim is null)
            {
                throw new NotFoundException(_localizer["RoleClaim Not Found"]);
            }

            existingRoleClaim.ClaimType = request.Type;
            existingRoleClaim.ClaimValue = request.Value;
            existingRoleClaim.Group = request.Group;
            existingRoleClaim.Description = request.Description;
            existingRoleClaim.RoleId = request.RoleId;
            _db.RoleClaims.Update(existingRoleClaim);
            await _db.SaveChangesAsync();

            return string.Format(_localizer["Role Claim {0} for Role updated."], request.Value);
        }
    }

    public async Task<string> DeleteAsync(int id)
    {
        var existingRoleClaim = await _db.RoleClaims
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existingRoleClaim is null)
        {
            throw new NotFoundException(_localizer["RoleClaim Not Found"]);
        }

        _db.RoleClaims.Remove(existingRoleClaim);
        await _db.SaveChangesAsync();

        return string.Format(_localizer["Role Claim {0} for Role deleted."], existingRoleClaim.ClaimValue);
    }
}