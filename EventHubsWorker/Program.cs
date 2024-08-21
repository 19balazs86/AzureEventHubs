using EventHubsWorker.Workers;
using Microsoft.Extensions.Azure;

namespace EventHubsWorker;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        IServiceCollection services  = builder.Services;
        IConfiguration configuration = builder.Configuration;

        string eventHubConnectionString = configuration.GetConnectionString("EventHubs")!;
        string storageConnectionString  = configuration.GetConnectionString("AzureWebJobsStorage")!;

        // Add services to the container
        {
            services.AddHostedService<ProducerWorker>();
            services.AddHostedService<ProcessorWorker>();

            services.AddAzureClients(clients =>
            {
                // Options to use SAS-ConnString:
                // 1) Use the hub SAS-ConnString directly
                // 2) Use the namespace SAS-ConnString, but you must provide the EventHubName
                clients.AddEventHubProducerClient(eventHubConnectionString);

                clients.AddBlobServiceClient(storageConnectionString);
            });
        }

        // Run the application
        IHost host = builder.Build();

        host.Run();
    }
}