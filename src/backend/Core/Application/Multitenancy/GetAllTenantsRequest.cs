namespace CodeMatrix.Mepd.Application.Multitenancy;

public class GetAllTenantsRequest : IRequest<List<TenantDto>>
{
}

internal class GetAllTenantsRequestHandler : IRequestHandler<GetAllTenantsRequest, List<TenantDto>>
{
    private readonly ITenantService _tenantService;

    public GetAllTenantsRequestHandler(ITenantService tenantService) => _tenantService = tenantService;

    public Task<List<TenantDto>> Handle(GetAllTenantsRequest request, CancellationToken cancellationToken) =>
        _tenantService.GetAllAsync();
}
