using System.Linq.Expressions;

namespace CodeMatrix.Mepd.Application.Common.Persistence;

// The Repository for the Application Db
// IRepositoryBase<T> is from Ardalis.Specification
public interface IRepository<T> : IRepositoryBase<T>
    where T : class, IAggregateRoot
{
}

/// <summary>
/// The read-only repository.
/// </summary>
public interface IReadRepository<T> : IReadRepositoryBase<T>
    where T : class, IAggregateRoot
{
    Task<bool> AnyAsync(ISpecification<T> specification = default, CancellationToken cancellationToken = default);

    Task<TResult> MaxAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        ISpecification<T> specification = default,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// A special (read/write) repository, that also adds events to the
/// entities domain events before adding, updating or deleting entities.
/// <typeparam name="T"></typeparam>
/// </summary>
public interface IRepositoryWithEvents<T> : IRepositoryBase<T>
    where T : class, IAggregateRoot
{ }