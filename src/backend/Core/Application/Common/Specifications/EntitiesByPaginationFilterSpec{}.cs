using CodeMatrix.Mepd.Application.Common.Models;

namespace CodeMatrix.Mepd.Application.Common.Specifications;

public class EntitiesByPaginationFilterSpec<T> : EntitiesByBaseFilterSpec<T>
{
    public EntitiesByPaginationFilterSpec(PaginationFilter filter)
        : base(filter) =>
        Query.PaginateBy(filter);
}