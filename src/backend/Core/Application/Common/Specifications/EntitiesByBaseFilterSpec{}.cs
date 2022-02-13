using Ardalis.Specification;
using CodeMatrix.Mepd.Application.Common.Models;

namespace CodeMatrix.Mepd.Application.Common.Specifications;

public class EntitiesByBaseFilterSpec<T> : Specification<T>
{
    public EntitiesByBaseFilterSpec(BaseFilter filter) =>
        Query.SearchBy(filter);
}
