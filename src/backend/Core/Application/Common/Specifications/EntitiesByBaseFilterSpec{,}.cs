using Ardalis.Specification;
using CodeMatrix.Mepd.Application.Common.Models;

namespace CodeMatrix.Mepd.Application.Common.Specifications;

public class EntitiesByBaseFilterSpec<T, TResult> : Specification<T, TResult>
{
    public EntitiesByBaseFilterSpec(BaseFilter filter) =>
        Query.SearchBy(filter);
}
