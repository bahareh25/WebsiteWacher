using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace WebsiteWacher.Services
{
    public class PdfCreatorService
    {
        public async Task<Stream> ConvertPageToPdfAsync(string url)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync(url);
            await page.EvaluateExpressionHandleAsync("document.fonts.ready");
            var pdfStream = await page.PdfStreamAsync();
            pdfStream.Position = 0;
            return pdfStream;
        }
    }
}
