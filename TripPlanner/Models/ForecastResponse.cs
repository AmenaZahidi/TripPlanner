using System.Text.Json.Serialization;

namespace TripPlanner.Models;

// The whole JSON reply from the Open-Meteo forecast API.
// Source: https://open-meteo.com/en/docs
public class ForecastResponse
{
    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("daily")]
    public DailyForecastData? Daily { get; set; }
}