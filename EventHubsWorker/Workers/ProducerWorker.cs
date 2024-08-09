using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using EventHubsWorker.Miscellaneous;

namespace EventHubsWorker.Workers;

public sealed class ProducerWorker : BackgroundService
{
    private readonly EventHubProducerClient _producerClient;

    public ProducerWorker(EventHubProducerClient producerClient)
    {
        _producerClient = producerClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            EventData[] events = createEventData(5).ToArray();

            using EventDataBatch eventBatch = await _producerClient.CreateBatchAsync();

            foreach (EventData eventData in events)
            {
                if (!eventBatch.TryAdd(eventData))
                {
                    throw new ApplicationException("Not all events could be added to the batch!");
                }
            }

            await _producerClient.SendAsync(eventBatch, stoppingToken);

            // await _producerClient.SendAsync(events, stoppingToken); // Simply send a list of events

            await Task.WhenAny(Task.Delay(10_000, stoppingToken));
        }
    }

    private static IEnumerable<EventData> createEventData(int number)
    {
        for (int i = 0; i < number; i++)
        {
            WeatherForecast weatherForecast = WeatherForecast.Create();

            BinaryData binaryData = BinaryData.FromObjectAsJson(weatherForecast);

            yield return new EventData(binaryData);
        }
    }
}
