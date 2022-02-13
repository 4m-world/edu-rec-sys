using CodeMatrix.Mepd.Application.Common.Validation;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CodeMatrix.Mepd.Application.Common.FileStorage;

public class FileUploadRequest
{
    public string Name { get; set; } = default;

    public string Extension { get; set; } = default;

    public string Data { get; set; } = default;
}

public class FileUploadRequestValidator : CustomValidator<FileUploadRequest>
{
    public FileUploadRequestValidator(IStringLocalizer<FileUploadRequestValidator> localizer)
    {
        RuleFor(p => p.Name).MaximumLength(150).NotEmpty().WithMessage(localizer["fileUpload.fileNameEmpty"]);
        RuleFor(p => p.Extension).MaximumLength(5).NotEmpty().WithMessage(localizer["fileUpload.fileExtEmpty"]);
        RuleFor(p => p.Data).NotEmpty().WithMessage(localizer["fileUpload.fileDataEmpty"]);
    }
}