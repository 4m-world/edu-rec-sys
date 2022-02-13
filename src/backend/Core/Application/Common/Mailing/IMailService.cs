using CodeMatrix.Mepd.Application.Common.Interfaces;

namespace CodeMatrix.Mepd.Application.Common.Mailing;

public interface IMailService : ITransientService
{
    Task SendAsync(MailRequest request);
}