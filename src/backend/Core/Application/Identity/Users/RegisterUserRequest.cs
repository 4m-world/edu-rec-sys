using CodeMatrix.Mepd.Application.Common.Validation;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CodeMatrix.Mepd.Application.Identity.Users;

public class RegisterUserRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    public string PhoneNumber { get; set; }
}

public class RegisterUserRequestValidator : CustomValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator(IUserService userService, IStringLocalizer<RegisterUserRequestValidator> localizer)
    {
        RuleFor(p => p.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(localizer["identity.invalidEmail"])
            .MustAsync(async (email, _) => !await userService.ExistsWithEmailAsync(email))
            .WithMessage((_, email) => string.Format(localizer["identity.alreadyExistsEmail"], email));

        RuleFor(p => p.Password).Cascade(CascadeMode.Stop).NotEmpty().MinimumLength(6);


        RuleFor(p => p.UserName).Cascade(CascadeMode.Stop).NotEmpty().MinimumLength(6)
             .MustAsync(async (name, _) => !await userService.ExistsWithNameAsync(name))
             .WithMessage((_, name) => string.Format(localizer["identity.alreadyExistsUsername"], name));

        RuleFor(u => u.PhoneNumber).Cascade(CascadeMode.Stop)
        .MustAsync(async (phone, _) => !await userService.ExistsWithNameAsync(phone!))
            .WithMessage((_, phone) => string.Format(localizer["identity.alreadyExistsPhone"], phone))
            .Unless(u => string.IsNullOrWhiteSpace(u.PhoneNumber));
         

        RuleFor(p => p.FirstName).Cascade(CascadeMode.Stop).NotEmpty();
        RuleFor(p => p.LastName).Cascade(CascadeMode.Stop).NotEmpty();
        RuleFor(p => p.ConfirmPassword).Cascade(CascadeMode.Stop).NotEmpty().Equal(e => e.Password);
    }
}