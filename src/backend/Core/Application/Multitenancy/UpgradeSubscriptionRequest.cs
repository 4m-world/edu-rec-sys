namespace CodeMatrix.Mepd.Application.Multitenancy;

public class UpgradeSubscriptionRequest : IRequest<string>
{
    public string TenantId { get; set; }
    public DateTime ExtendedExpiryDate { get; set; }
}

public class UpgradeSubscriptionRequestValidator : CustomValidator<UpgradeSubscriptionRequest>
{
    public UpgradeSubscriptionRequestValidator() =>
        RuleFor(t => t.TenantId)
            .NotEmpty();
}

internal class UpgradeSubscriptionRequestHandler : IRequestHandler<UpgradeSubscriptionRequest, string>
{
    private readonly ITenantService _tenantService;

    public UpgradeSubscriptionRequestHandler(ITenantService tenantService) => _tenantService = tenantService;

    public Task<string> Handle(UpgradeSubscriptionRequest request, CancellationToken cancellationToken) =>
        _tenantService.UpdateSubscription(request.TenantId, request.ExtendedExpiryDate, cancellationToken);
}