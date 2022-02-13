using CodeMatrix.Mepd.Application.Common.Validation;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CodeMatrix.Mepd.Application.Identity.Roles;

public class RoleUpdateRequest
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class RoleRequestValidator : CustomValidator<RoleUpdateRequest>
{
    private readonly IRoleService _roleService = default!;
    private readonly IStringLocalizer<RoleRequestValidator> _localizer;

    public RoleRequestValidator(IRoleService roleService, IStringLocalizer<RoleRequestValidator> localizer)
    {
        _roleService = roleService;
        _localizer = localizer;

        RuleFor(r => r.Name)
            .NotEmpty()
            .MustAsync(async (role, name, _) => !await _roleService.ExistsAsync(name, role.Id))
                .WithMessage(_localizer["role.alreadyExists"]);
    }
}