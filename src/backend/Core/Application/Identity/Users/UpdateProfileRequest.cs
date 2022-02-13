using CodeMatrix.Mepd.Application.Common.FileStorage;
using CodeMatrix.Mepd.Application.Common.Validation;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CodeMatrix.Mepd.Application.Identity.Users;

public class UpdateProfileRequest 
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public FileUploadRequest Image { get; set; }
}

public class UpdateProfileRequestValidator : CustomValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator(
        IUserService userService,
        IStringLocalizer<UpdateProfileRequestValidator> localizer,
        IStringLocalizer<FileUploadRequestValidator> fileLocalizer)
    {
        RuleFor(p => p.Id).NotEmpty();

        RuleFor(p => p.FirstName).Cascade(CascadeMode.Stop).NotEmpty().MaximumLength(75);
        RuleFor(p => p.LastName).Cascade(CascadeMode.Stop).NotEmpty().MaximumLength(75);
        RuleFor(p => p.Email).Cascade(CascadeMode.Stop).NotEmpty().WithMessage(localizer["identity.invalidEmail"])
            .MustAsync(async (user, email, _) => !await userService.ExistsWithEmailAsync(email, user.Id))
                .WithMessage((_, email) => string.Format(localizer["identity.alreadyExistsEmail"], email));

        RuleFor(p => p.Image).SetNonNullableValidator(new FileUploadRequestValidator(fileLocalizer));

        RuleFor(u => u.PhoneNumber).Cascade(CascadeMode.Stop)
           .MustAsync(async (user, phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!, user.Id))
               .WithMessage((_, phone) => string.Format(localizer["identity.alreadyExistsPhone"], phone))
               .Unless(u => string.IsNullOrWhiteSpace(u.PhoneNumber));
    }
}