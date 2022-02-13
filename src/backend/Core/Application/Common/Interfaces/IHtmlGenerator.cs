namespace CodeMatrix.Mepd.Application.Common.Interfaces;

public interface IHtmlGenerator
{
    Task<string> GenerateHtml<T>(string template, T model)
        where T : class;
}
