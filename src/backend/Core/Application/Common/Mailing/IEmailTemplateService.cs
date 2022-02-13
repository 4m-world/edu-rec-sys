using CodeMatrix.Mepd.Application.Common.Interfaces;

namespace CodeMatrix.Mepd.Application.Common.Mailing;

public interface IEmailTemplateService : ITransientService
{
    string GenerateEmailConfirmationMail(string userName, string email, string emailVerificationUri);
    string GenerateEmailFromTemlateMail(Dictionary<string, string> args, string templateName);
}