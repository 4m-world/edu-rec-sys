namespace CodeMatrix.Mepd.Application.Common.Models;

public class PaginationResponse<T>
{
    public PaginationResponse(List<T> data)
    {
        Data = data;
    }
    public PaginationResponse(List<T> data, int count, int page, int pageSize)
            : this(true, data, default, count, page, pageSize) { }

    public List<T> Data { get; set; }

    internal PaginationResponse(bool succeeded, List<T> data = default, List<string> messages = null, int count = 0, int page = 1, int pageSize = 10)
    {
        Data = data;
        CurrentPage = page;
        Succeeded = succeeded;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Messages = messages;
    }

    public static PaginationResponse<T> Failure(List<string> messages)
    {
        return new(false, default, messages);
    }

    public static PaginationResponse<T> Success(List<T> data, int count, int page, int pageSize)
    {
        return new(true, data, null, count, page, pageSize);
    }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;


    // kept for backword compatablity
    public List<string> Messages { get; set; } = new();
    // kept for backword compatablity
    public bool Succeeded { get; set; }

}