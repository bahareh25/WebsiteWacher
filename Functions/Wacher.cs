using System;
using HtmlAgilityPack;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using WebsiteWacher.Services;

namespace WebsiteWacher.Functions;

public class Wacher(ILoggerFactory loggerFactory, PdfCreatorService pdfCreatorService)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<Wacher>();
    private const string SqlInputQuery = @"SELECT w.Id, w.Url, w.xPathExpression, s.Content FROM dbo.WebSites w LEFT JOIN 
                                        dbo.Snapshots s ON w.Id = s.Id
                                        WHERE s.Timestamp=(Select Max(Timestamp) from dbo.Snapshots where Id=w.id)";
    [Function(nameof(Wacher))]

    public async Task Run([TimerTrigger("*/20 * * * * *")] TimerInfo myTimer, [SqlInput(SqlInputQuery, "WebsiteWacher")] IReadOnlyList<WebsiteModel> websites)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

        foreach (var website in websites)
        {
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(website.Url);
            var divewithcontent = doc.DocumentNode.SelectSingleNode(website.xPathExpression);
            var content = divewithcontent != null ? divewithcontent.InnerHtml.Trim() : "No Content";
            content = content.Replace("Microsoft Entra", "Azure AD");
            var contentChanged = content != website.LatestContext;
            if (contentChanged)
            {
                _logger.LogInformation("content changed");
                var newPdfStream = await pdfCreatorService.ConvertPageToPdfAsync(website.Url);
                var connectionstring = Environment.GetEnvironmentVariable("ConnectionStrings:WebSiteWacherStorage");
                var blobclient = new Azure.Storage.Blobs.BlobClient(connectionstring, "pdfs", $"{website.id}-{DateTime.UtcNow:MMddyyyyhhmmss}.pdf");
                await blobclient.UploadAsync(newPdfStream);
                _logger.LogInformation($"PDF created for URL: {website.Url}, PDF Size: {newPdfStream.Length} bytes");
            }
        }
    }

    public class WebsiteModel
    {
        public Guid id { get; set; }
        public string Url { get; set; }
        public string xPathExpression { get; set; }
        public string LatestContext { get; set; }
    }
}