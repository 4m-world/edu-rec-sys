using CodeMatrix.Mepd.Application.Identity.Roles;
using CodeMatrix.Mepd.Infrastructure.Identity;
using Mapster;

namespace CodeMatrix.Mepd.Infrastructure.Mapping;

public class MapsterSettings
{
    public static void Configure()
    {
        // here we will define the type conversion / Custom-mapping
        // More details at https://github.com/MapsterMapper/Mapster/wiki/Custom-mapping

        // This is used in UserService.GetPermissionsAsync
        TypeAdapterConfig<ApplicationRoleClaim, PermissionDto>.NewConfig().Map(dest => dest.Permission, src => src.ClaimValue);
    }
}