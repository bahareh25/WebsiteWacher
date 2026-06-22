using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using WebsiteWacher;
using WebsiteWacher.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(app => {
    app.UseWhen<SafeBrowsingMiddleware>(context => { return context.FunctionDefinition.Name == "Register"; });
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<PdfCreatorService>();
        services.AddSingleton<SafeBrowsingService>();
        services.AddOpenTelemetry()
            .UseFunctionsWorkerDefaults()
            .UseAzureMonitorExporter(options =>
            {
                options.ConnectionString = "InstrumentationKey=your-key-here";
            });
    })
    .Build();

host.Run();

//var builder = FunctionsApplication.CreateBuilder(args);
////builder.ConfigureFunctionsWorkerDefaults(workerOptions =>
////{
////    workerOptions.UseMiddleware<SafeBrowsingMiddelware>();
////});

//builder.ConfigureFunctionsWebApplication();
//builder.Logging.AddConsole();
//builder.Logging.SetMinimumLevel(LogLevel.Information);
//builder.Services.AddOpenTelemetry()
//    .UseFunctionsWorkerDefaults()
//    .UseAzureMonitorExporter(options =>
//    {
//        options.ConnectionString = "InstrumentationKey=your-key-here";
//    });
//builder.Services.AddSingleton<PdfCreatorService>();
//builder.Services.AddSingleton<SafeBrowsingService>();
//builder.Build().Run();
