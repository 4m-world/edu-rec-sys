using CodeMatrix.Mepd.Application.Common.Mailing;
using Microsoft.Extensions.Localization;
using System.Text;

namespace CodeMatrix.Mepd.Infrastructure.Mailing;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IStringLocalizer<EmailTemplateService> _localizer;

    public EmailTemplateService(IStringLocalizer<EmailTemplateService> localizer)
    {
        _localizer = localizer;
    }

    public string GenerateEmailConfirmationMail(string userName, string email, string emailVerificationUri)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string tmplFolder = Path.Combine(baseDirectory, "EmailTemplates");
        string filePath = Path.Combine(tmplFolder, "email-confirmation.html");

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        string mailText = sr.ReadToEnd();
        sr.Close();

        if (string.IsNullOrEmpty(mailText))
        {
            return string.Format(_localizer["Please confirm your account by <a href='{0}'>clicking here</a>."], emailVerificationUri);
        }

        return mailText.Replace("[userName]", userName, StringComparison.OrdinalIgnoreCase)
            .Replace("[email]", email, StringComparison.OrdinalIgnoreCase)
            .Replace("[emailVerificationUri]", emailVerificationUri, StringComparison.OrdinalIgnoreCase);
    }

    public string GenerateEmailFromTemlateMail(Dictionary<string, string> args, string templateName)
    {
        if (args == null) throw new ArgumentNullException(nameof(args));
        
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var tmplFolder = Path.Combine(baseDirectory, "EmailTemplates");
        var filePath = Path.Combine(tmplFolder, $"{templateName}.html");

        if (!File.Exists(filePath))
            throw new FileNotFoundException(_localizer["Template file not found"], templateName);

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        var templateContent = sr.ReadToEnd();
        sr.Close();

        if (string.IsNullOrWhiteSpace(templateContent))
        {
            throw new InvalidDataException(_localizer["No template content found to render"]);
        }

        foreach (var key in args.Keys)
        {
            templateContent = templateContent.Replace($"[{key}]", args[key], StringComparison.OrdinalIgnoreCase);
        }

        return templateContent;

    }
}