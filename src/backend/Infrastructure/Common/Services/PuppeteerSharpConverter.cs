using CodeMatrix.Mepd.Application.Common.Interfaces;
using PuppeteerSharp;

namespace CodeMatrix.Mepd.Infrastructure.Common.Services
{
    public class PuppeteerSharpConverter : IPdfConverter
    {
        public async Task<Stream> ConvertAasync(string html, Application.Common.Interfaces.PdfOptions options = null)
        {
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);

            return new MemoryStream(await page.PdfDataAsync(new global::PuppeteerSharp.PdfOptions
            {
                PrintBackground = true,
            }));
        }
    }
}
