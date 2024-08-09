using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using EventHubsWorker.Miscellaneous;

namespace EventHubsWorker.Workers;

public sealed class ProcessorWorker : IHostedService
{
    private readonly string _connectionString;

    private const string _consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

    private readonly ILogger<ProcessorWorker> _logger;

    private readonly BlobContainerClient _blobContainerClient;

    private EventProcessorClient? _processorClient;

    public ProcessorWorker(ILogger<ProcessorWorker> logger, BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _logger = logger;

        _blobContainerClient = blobServiceClient.GetBlobContainerClient("event-hub-processor");

        _connectionString = configuration.GetConnectionString("EventHubs")!;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        await _blobContainerClient.CreateIfNotExistsAsync();

        _processorClient = new EventProcessorClient(_blobContainerClient, _consumerGroup, _connectionString);

        _processorClient.ProcessEventAsync += processEventHandler;
        _processorClient.ProcessErrorAsync += processErrorHandler;

        await _processorClient.StartProcessingAsync(ct);
    }

    public async Task StopAsync(CancellationToken ct)
    {
        if (_processorClient is null)
        {
            return;
        }

        await _processorClient.StopProcessingAsync(ct);

        _processorClient.ProcessEventAsync -= processEventHandler;
        _processorClient.ProcessErrorAsync -= processErrorHandler;
    }

    // EventProcessorClient can process multiple events in parallel, but with some limitations.
    // Each instance of EventProcessorClient handle events from multiple partitions concurrently.
    // However, for a given partition, events are processed one at a time.
    private async Task processEventHandler(ProcessEventArgs args)
    {
        try // Suggested to guard against exceptions with try-catch
        {
            if (args.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (Random.Shared.NextDouble() < 0.01)
            {
                throw new Exception("Random exception");
            }

            string partition = args.Partition.PartitionId;

            WeatherForecast weatherForecast = args.Data.EventBody.ToObjectFromJson<WeatherForecast>();

            _logger.LogInformation("--> Event from partition {partition}. WeatherForecast in {city} with summary: {summary}.",
                partition, weatherForecast.City, weatherForecast.Summary);

            await args.UpdateCheckpointAsync();

            await Task.WhenAny(Task.Delay(1_500, args.CancellationToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while processing an event");

            // throw; // There is no need to throw any exception in the handler, as there are NO automatic retries when an exception occurs
        }
    }

    private Task processErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error in the EventProcessorClient operation {operation}", args.Operation);

        return Task.CompletedTask;
    }
}
