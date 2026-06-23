using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using WebsiteWacher.Models;
using Microsoft.Extensions.Hosting;
using PuppeteerSharp;
using WebsiteWacher.Services;


namespace WebsiteWacher.Functions;

public class PdfCreator(ILoggerFactory loggerFactory, PdfCreatorService pdfCreatorService)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<PdfCreator>();

    [Function(nameof(PdfCreator))]

    public async Task Run(
        [SqlTrigger("dbo.Websites", "WebsiteWacher")] SqlChange<WebSite>[] changes)
    {

        foreach (var change in changes)
        {

            if (change.Operation == SqlChangeOperation.Insert)
            {
                var pdfStream = await pdfCreatorService.ConvertPageToPdfAsync(change.Item.Url);

                _logger.LogInformation($"PDF created for URL: {change.Item.Url}, PDF Size: {pdfStream.Length} bytes");
                var connectionstring = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var blobclient = new Azure.Storage.Blobs.BlobClient(connectionstring, "pdfs", $"{change.Item.id}.pdf");
                await blobclient.UploadAsync(pdfStream);
            }
        }

    }

    //for bloboutput
    //[BlobOutput("pdfs/new1.pdf", Connection = "WebSiteWacherStorage")]
    //public async Task<byte[]?> Run(
    //   [SqlTrigger("dbo.Websites", "WebsiteWacher")] SqlChange<WebSite>[] changes)
    //{
    //    byte[]? pdfBytes = null;
    //    foreach (var change in changes)
    //    {

    //        if (change.Operation == SqlChangeOperation.Insert)
    //        {
    //            var pdfStream = await ConvertPageToPdfAsync(change.Item.Url);
    //            pdfBytes = new byte[pdfStream.Length];
    //            await pdfStream.ReadAsync(pdfBytes, 0, (int)pdfStream.Length);
    //            _logger.LogInformation($"PDF created for URL: {change.Item.Url}, PDF Size: {pdfStream.Length} bytes");
    //            var connectionstring = Environment.GetEnvironmentVariable("ConnectionStrings:WebSiteWacherStorage");
    //            var blobclient = new Azure.Storage.Blobs.BlobClient(connectionstring, "pdfs", $"{change.Item.id}.pdf");
    //            await blobclient.UploadAsync(pdfStream);
    //        }
    //    }
    //    return pdfBytes;
    //}
}
