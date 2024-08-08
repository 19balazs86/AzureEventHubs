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

        string connectionString = configuration.GetConnectionString("EventHubs")!;

        // Add services to the container
        {
            services.AddHostedService<ProducerWorker>();

            // You can add the producer-client with the hub SAS-ConnString
            // BUT: If you use the namespace SAS-ConnString, you need to provide the EventHubName, when you add producer-client
            services.AddAzureClients(clients => clients.AddEventHubProducerClient(connectionString));
        }

        // Run the application
        IHost host = builder.Build();

        host.Run();
    }
}