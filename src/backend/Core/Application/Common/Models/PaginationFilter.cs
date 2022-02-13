namespace CodeMatrix.Mepd.Application.Common.Models;

public class PaginationFilter : BaseFilter, IPaginationFilter
{
    private int _pageNumber;

    protected PaginationFilter()
    {
        PageSize = int.MaxValue;
    }

    public PaginationFilter(int pageNumber, int pageSize, string[] orderBy = default)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize;
        OrderBy = orderBy;
    }

    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            _pageNumber = value < 1 ? 1 : value;
        }
    }

    public int PageSize { get; set; } = int.MaxValue;

    public string[] OrderBy { get; set; }
}

public class BasicPaginationFilter : IPaginationFilter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; } = int.MaxValue;
    public string[] OrderBy { get; set; }
}

public interface IPaginationFilter
{
    int PageNumber { get; }
    int PageSize { get; }
    public string[] OrderBy { get; }
}

public static class PaginationFilterExtensions
{
    public static bool HasOrderBy(this IPaginationFilter filter)
    {
        return filter.OrderBy?.Any() is true;
    }
}