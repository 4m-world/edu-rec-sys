using CodeMatrix.Mepd.Application.Common.Validation;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CodeMatrix.Mepd.Application.Identity.Roles;

public class RoleCreateRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class RoleCreateRequestValidator : CustomValidator<RoleCreateRequest>
{
    private readonly IRoleService _roleService = default!;
    private readonly IStringLocalizer<RoleCreateRequestValidator> _localizer;

    public RoleCreateRequestValidator(IRoleService roleService, IStringLocalizer<RoleCreateRequestValidator> localizer)
    {
        _roleService = roleService;
        _localizer = localizer;

        RuleFor(r => r.Name)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(100)            
            .MustAsync(async (role, name, _) => !await _roleService.ExistsAsync(name, default))
                .WithMessage(_localizer["role.alreadyExists"]);
    }
}