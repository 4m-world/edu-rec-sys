using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Application.Common.Models;
using MediatR;

namespace CodeMatrix.Mepd.Application.Auditing;

/// <summary>
/// Get current user audit logs request
/// </summary>
public class GetMyAuditLogsRequest : PaginationFilter, IRequest<PaginationResponse<AuditDto>>
{

}

internal class GetMyAuditLogsRequestHandler : IRequestHandler<GetMyAuditLogsRequest, PaginationResponse<AuditDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuditService _auditService;

    public GetMyAuditLogsRequestHandler(ICurrentUser currentUser, IAuditService auditService)
    {
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public Task<PaginationResponse<AuditDto>> Handle(GetMyAuditLogsRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        return _auditService.GetUserTrailsAsync(userId, request.Keyword, request.PageNumber, request.PageSize, request.OrderBy, cancellationToken);
    }
}