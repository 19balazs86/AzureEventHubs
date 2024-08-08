using EventHubsWorker.Workers;
using Microsoft.Extensions.Azure;

namespace EventHubsWorker;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;

        string connectionString = configuration.GetConnectionString("EventHubs")!;

        // Add services to the container
        {
            services.AddHostedService<ProducerWorker>();

            services.AddAzureClients(clients => clients.AddEventHubProducerClient(connectionString));
        }

        // Run the application
        IHost host = builder.Build();

        host.Run();
    }
}