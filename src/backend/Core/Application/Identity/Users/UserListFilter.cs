using CodeMatrix.Mepd.Application.Common.Models;

namespace CodeMatrix.Mepd.Application.Identity.Users;

public class UserListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
}
