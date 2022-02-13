using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using CodeMatrix.Mepd.Application.Common.Persistence;
using CodeMatrix.Mepd.Domain.Common.Contracts;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CodeMatrix.Mepd.Infrastructure.Persistence.Repository;


// Inherited from Ardalis.Specification's RepositoryBase<T>
public class ApplicationDbRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    public ApplicationDbRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    // AnyAsync is in the pipeline in Ardalis.Specifications... so this is temporary...
    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken) =>
        await ApplySpecification(specification, true)
            .AnyAsync(cancellationToken);

    public virtual async Task<TResult> MaxAsync<TResult>(
        Expression<Func<T, TResult>> selector, 
        ISpecification<T> specification = null,
        CancellationToken cancellationToken = default)
    {
       return await ApplySpecification(specification, true)
             .MaxAsync(selector, cancellationToken);
    }

    // We override the default behavior when mapping to a dto.
    // We're using Mapster's ProjectToType here to immediately map the result from the database.
    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        ApplySpecification(specification, false)
            .ProjectToType<TResult>();
}
