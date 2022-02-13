using CodeMatrix.Mepd.Application.Common.Models;

namespace CodeMatrix.Mepd.Application.Identity.Users;

public class UsersFilter : BasicPaginationFilter
{
    public bool? IsActive { get; set; }
    public string Search { get; set; }
}