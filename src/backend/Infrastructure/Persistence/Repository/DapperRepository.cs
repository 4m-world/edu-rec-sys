using CodeMatrix.Mepd.Application.Common.Exceptions;
using CodeMatrix.Mepd.Application.Common.Models;
using CodeMatrix.Mepd.Application.Common.Persistence;
using CodeMatrix.Mepd.Domain.Common.Contracts;
using CodeMatrix.Mepd.Infrastructure.Mapping;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using Dapper;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using System.Data;

namespace CodeMatrix.Mepd.Infrastructure.Persistence.Repository;

public class DapperRepository : IDapperRepository
{
    private readonly ApplicationDbContext _dbContext;

    public DapperRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
    where T : class =>
        (await _dbContext.Connection.QueryAsync<T>(sql, param, transaction))
            .AsList();

    public async Task<PaginationResponse<T>> QueryAsync<T>(string sql, int pageNumber, int pageSize, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var queryResult = await _dbContext.Connection.QueryAsync<T>(sql, param, transaction);
        var result = queryResult.AsQueryable().ToMappedPaginatedResult<T, T>(pageNumber, pageSize);
        return result;
    }

    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        where T : class//, IEntity
    {
        if (_dbContext.Model.GetMultiTenantEntityTypes().FirstOrDefault(t => t.ClrType == typeof(T)) is not null)
        {
            sql = sql.Replace("@tenant", _dbContext.TenantInfo.Id, StringComparison.OrdinalIgnoreCase);
        }

        var entity = await _dbContext.Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);

        return entity ?? throw new NotFoundException(string.Empty);
    }

    public Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        where T : class, IEntity
    {
        if (_dbContext.Model.GetMultiTenantEntityTypes().FirstOrDefault(t => t.ClrType == typeof(T)) is not null)
        {
            sql = sql.Replace("@tenant", _dbContext.TenantInfo.Id, StringComparison.OrdinalIgnoreCase);
        }

        return _dbContext.Connection.QuerySingleAsync<T>(sql, param, transaction);
    }
}