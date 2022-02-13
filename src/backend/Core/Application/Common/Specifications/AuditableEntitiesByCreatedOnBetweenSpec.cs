using Ardalis.Specification;
using CodeMatrix.Mepd.Domain.Common.Contracts;

namespace CodeMatrix.Mepd.Application.Common.Specifications;

public class AuditableEntitiesByCreatedOnBetweenSpec<T> : Specification<T>
    where T : AuditableEntity
{
    public AuditableEntitiesByCreatedOnBetweenSpec(DateTime from, DateTime until) =>
        Query.Where(e => e.CreatedOn >= from && e.CreatedOn <= until);
}
