using CodeMatrix.Mepd.Domain.Dump;

namespace CodeMatrix.Mepd.Application.Dump;

public class GetStudyMajorByCodeSpec : Specification<StudyMajor>, ISingleResultSpecification
{
    public GetStudyMajorByCodeSpec(string code)
    {
        Query.Where(e => e.Code.Trim().ToLower() == code.ToLower());
    }
}
