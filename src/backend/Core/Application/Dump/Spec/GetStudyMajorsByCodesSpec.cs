using CodeMatrix.Mepd.Domain.Dump;

namespace CodeMatrix.Mepd.Application.Dump;

public class GetStudyMajorsByCodesSpec : EntitiesByPaginationFilterSpec<StudyMajor>
{
    public GetStudyMajorsByCodesSpec(string[] codes, int page = 1, int pageSize = int.MaxValue)
        : base(new PaginationFilter(page, pageSize))
    {
        Query.Where(e => codes.Contains(e.Code));
        Query.OrderByDescending(e => e.CreatedOn);
    }
}
