namespace CodeMatrix.Mepd.Application.Identity.Roles;

public class RoleDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsRootRole { get; set; } = false;
    public List<PermissionGroup>? Permissions { get; set; }
}