using CodeMatrix.Mepd.Application.Common.Validation;
using FluentValidation;

namespace CodeMatrix.Mepd.Application.Identity.Users.Password;

public class ChangePasswordRequest
{
    public string Password { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}

public class ChangePasswordRequestValidator : CustomValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator(IStringLocalizer<ChangePasswordRequestValidator> localizer)
    {
        RuleFor(p => p.Password)
            .NotEmpty();

        RuleFor(p => p.NewPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(6)
            .NotEqual(e=> e.Password).WithMessage(localizer["changePassword.shouldNotEqualPassword"]);

        RuleFor(p => p.ConfirmNewPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Equal(p => p.NewPassword).WithMessage(localizer["changePassword.confirmPwdDoNotMatch"]);
    }
}