namespace CodeMatrix.Mepd.Application.Identity.Roles;

public class PermissionDto
{
    public string Key { get; set; }
    public string Permission { get; set; }
    public string Description { get; set; }
    public bool IsGranted { get; set; }
}
