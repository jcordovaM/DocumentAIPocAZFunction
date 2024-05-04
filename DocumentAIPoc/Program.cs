using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DocumentAIPoc.Interfaces;
using DocumentAIPoc.Services;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddTransient<IFileManager, FileManager>();
        services.AddTransient<IDocumentService, DocumentService>();
    })
    .ConfigureAppConfiguration(app =>
    {
        app.AddJsonFile("./appsettings.json");
    })
    .Build();

host.Run();
