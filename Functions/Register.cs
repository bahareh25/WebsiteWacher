using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using WebsiteWacher.Models;
using WebsiteWacher.Services;

namespace WebsiteWacher.Functions;

public class Register(ILogger<Register> logger)// SafeBrowsingService safeBrowsingService
{
    private readonly ILogger<Register> _logger = logger;

    [Function(nameof(Register))]
    [SqlOutput("dbo.WebSites", "WebsiteWacher")]
    public async Task<WebSite> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var newWebsite = JsonSerializer.Deserialize<WebSite>(requestBody, options);
        newWebsite.id = Guid.NewGuid();
        //var result=safeBrowsingService.CheckUrl(newWebsite.Url);
        //if(result.HasThreat)
        //{
        //    var threats=string.Join(", ", result.ThreatType);
        //    _logger.LogError($"URL {newWebsite.Url} is flagged as unsafe. Threat type: {result.ThreatType}");
        //    return null;
        //}
        return newWebsite;
    }
}