namespace EventHubsWorker.Miscellaneous;

public sealed class WeatherForecast
{
    private static readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private static readonly string[] _cities =
    [
        "London", "Budapest", "Paris", "Dublin", "Zurich", "Los Angeles", "New York"
    ];

    public required string City { get; init; }
    public DateTime Date { get; init; }
    public int Temperature { get; init; }
    public required string Summary { get; init; }

    public static WeatherForecast Create()
    {
        return new WeatherForecast
        {
            City        = _cities[Random.Shared.Next(_cities.Length)],
            Date        = DateTime.UtcNow.AddDays(1),
            Temperature = Random.Shared.Next(-20, 55),
            Summary     = _summaries[Random.Shared.Next(_summaries.Length)]
        };
    }
}

