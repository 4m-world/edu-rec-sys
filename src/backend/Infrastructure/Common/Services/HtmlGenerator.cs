using CodeMatrix.Mepd.Application.Common.Interfaces;
using RazorLight;

namespace CodeMatrix.Mepd.Infrastructure.Common.Services
{
    public class HtmlGenerator : IHtmlGenerator
    {
        private readonly IRazorLightEngine _razorLightEngine;

        public HtmlGenerator(IRazorLightEngine razorLightEngine)
        {
            _razorLightEngine = razorLightEngine;
        }

        public Task<string> GenerateHtml<T>(string template, T model) where T : class
        {
            return _razorLightEngine.CompileRenderAsync(template, model);
        }
    }
}
