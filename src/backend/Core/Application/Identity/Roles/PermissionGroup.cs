namespace CodeMatrix.Mepd.Application.Identity.Roles;

public class PermissionGroup
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<PermissionDto> Permissions { get; set; }
}