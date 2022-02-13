using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Application.Common.Models;

namespace CodeMatrix.Mepd.Application.Auditing;

/// <summary>
/// Audit service contract
/// </summary>
public interface IAuditService : ITransientService
{
    /// <summary>
    /// Get user trail
    /// </summary>
    public Task<PaginationResponse<AuditDto>> GetUserTrailsAsync(
        string userId, 
        string search, 
        int pageNumber, 
        int pageSize = 10, string[] orderBy = default, 
        CancellationToken cancellationToken = default);
}