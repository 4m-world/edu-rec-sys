using CodeMatrix.Mepd.Application.Common.Validation;
using FluentValidation;

namespace CodeMatrix.Mepd.Application.Identity.Users.Password;

public class ForgotPasswordRequest
{
    public string Email { get; set; }
}

public class ForgotPasswordRequestValidator : CustomValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(p => p.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress().WithMessage("identity.invalidEmail");
    }
}