using CodeMatrix.Mepd.Application.Auditing;
using CodeMatrix.Mepd.Application.Common.Models;
using CodeMatrix.Mepd.Infrastructure.Mapping;
using CodeMatrix.Mepd.Infrastructure.Persistence;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CodeMatrix.Mepd.Infrastructure.Auditing;

/// <summary>
/// Audit trail service
/// </summary>
public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initalize service
    /// </summary>
    /// <param name="context">Application db context</param>
    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get user trails
    /// </summary>
    public async Task<PaginationResponse<AuditDto>> GetUserTrailsAsync(string userId, string search, int pageNumber, int pageSize = 10, string[] orderBy = default, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditTrails.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.SearchByKeyword(search);
        }

        query = query.Where(e => e.UserId == userId);
        query = query.OrderByDescending(e => e.DateTime).ThenBy(e => e.TableName);
        var trails = await query.ToMappedPaginatedResultAsync<Trail, AuditDto>(pageNumber, pageSize, cancellationToken);
        return trails;
    }
}