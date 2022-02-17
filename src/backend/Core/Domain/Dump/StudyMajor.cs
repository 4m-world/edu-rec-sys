using CodeMatrix.Mepd.Domain.Common.Contracts;

namespace CodeMatrix.Mepd.Domain.Dump;

public class StudyMajor : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
}
