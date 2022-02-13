using CodeMatrix.Mepd.Shared.DTOs.Common.Contracts;
using MassTransit;

namespace CodeMatrix.Mepd.Domain.Common.Contracts;

public abstract class AuditableEntity<T> : BaseEntity<T>, IAuditableEntity, ISoftDelete
{
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; private set; }
    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string DeletedBy { get; set; }

    protected AuditableEntity()
    {
        CreatedOn = DateTime.UtcNow;
        LastModifiedOn = DateTime.UtcNow;
    }
}

public abstract class AuditableEntity : AuditableEntity<string>
{
    public AuditableEntity()
    {
        Id = NewId.Next().ToGuid().ToString();
    }
}