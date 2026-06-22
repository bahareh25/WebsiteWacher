using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using static WebsiteWacher.Functions.Wacher;

namespace WebsiteWacher.Functions;

public class Query
{
    private readonly ILogger<Query> _logger;
    private const string SqlInputQuery = @"SELECT w.Id, w.Url, s.[TimeStamp] as LastTimestamp FROM dbo.WebSites w LEFT JOIN 
                                        dbo.Snapshots s ON w.Id = s.Id
                                        WHERE s.Timestamp=(Select Max(Timestamp) from dbo.Snapshots where Id=w.id)
                                        AND s.Timestamp Between DATEADD(hour,-3,GETUTCDATE()) and GETUTCDATE()";
    public Query(ILogger<Query> logger)
    {
        _logger = logger;
    }

    [Function(nameof(Query))]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, [SqlInput(SqlInputQuery, "WebsiteWacher")] IReadOnlyList<dynamic> websites)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(websites);
    }
}