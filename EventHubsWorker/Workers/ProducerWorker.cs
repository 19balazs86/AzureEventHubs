using Azure.Messaging.EventHubs.Producer;

namespace EventHubsWorker.Workers;

public sealed class ProducerWorker : BackgroundService
{
    private readonly ILogger<ProducerWorker> _logger;

    private readonly EventHubProducerClient _eventHubProducerClient;

    public ProducerWorker(ILogger<ProducerWorker> logger, EventHubProducerClient eventHubProducerClient)
    {
        _logger = logger;

        _eventHubProducerClient = eventHubProducerClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1_000);
        }
    }
}
