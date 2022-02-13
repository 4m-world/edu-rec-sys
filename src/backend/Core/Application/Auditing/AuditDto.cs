using CodeMatrix.Mepd.Application.Common.Interfaces;

namespace CodeMatrix.Mepd.Application.Auditing;

/// <summary>
/// Audit Trial DTO
/// </summary>
public class AuditDto : IDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Type { get; set; } = default;
    public string TableName { get; set; } = default;
    public DateTime DateTime { get; set; }
    public string OldValues { get; set; } = default;
    public string NewValues { get; set; } = default;
    public string AffectedColumns { get; set; } = default;
    public string PrimaryKey { get; set; } = default;
}
