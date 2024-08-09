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
                // You can add the producer-client with the hub SAS-ConnString
                // BUT: If you use the namespace SAS-ConnString, you need to provide the EventHubName, when you add producer-client
                clients.AddEventHubProducerClient(eventHubConnectionString);

                clients.AddBlobServiceClient(storageConnectionString);
            });
        }

        // Run the application
        IHost host = builder.Build();

        host.Run();
    }
}