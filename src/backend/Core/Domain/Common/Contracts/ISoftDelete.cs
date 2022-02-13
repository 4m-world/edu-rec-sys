namespace CodeMatrix.Mepd.Shared.DTOs.Common.Contracts;

public interface ISoftDelete
{
    DateTime? DeletedOn { get; set; }
    string DeletedBy { get; set; }
}