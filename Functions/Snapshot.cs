using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;


namespace WebsiteWacher.Functions;

public class Snapshot
{
    private readonly ILogger _logger;

    public Snapshot(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Snapshot>();
    }

    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    [Function(nameof(Snapshot))]
    [SqlOutput("dbo.Snapshots", "WebsiteWacher")]
    public SnapshotRecord? Run(
        [SqlTrigger("dbo.Websites", "WebsiteWacher")] IReadOnlyList<SqlChange<Models.WebSite>> changes
         )
    {
        SnapshotRecord? snapshotRecord = null;
        foreach (var change in changes)
        {
            _logger.LogInformation($"{change.Operation}");
            _logger.LogInformation($"Id: {change.Item.id} Url:{change.Item.Url}");
            if (change.Operation != SqlChangeOperation.Insert)
            {
                continue;
            }
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(change.Item.Url);
            var divewithcontent = doc.DocumentNode.SelectSingleNode(change.Item.xPathExpression);
            var content = divewithcontent != null ? divewithcontent.InnerHtml.Trim() : "No Content";
            _logger.LogInformation(content);
            snapshotRecord = new SnapshotRecord(change.Item.id, content);
        }
        return snapshotRecord;
    }
    public record SnapshotRecord(Guid Id, string Content);
}

