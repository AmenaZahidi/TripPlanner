using System.Text.Json.Serialization;

namespace TripPlanner.Models;

// The daily part of the forecast JSON.
// The API sends one list per value, and the item at the same position
// in each list belongs to the same day (e.g. item 0 in every list = day 1).
// The values are nullable because the API can send null if it has no data for a day.
public class DailyForecastData
{
    [JsonPropertyName("time")]
    public List<string> Dates { get; set; } = new();

    [JsonPropertyName("weather_code")]
    public List<int?> WeatherCodes { get; set; } = new();

    [JsonPropertyName("temperature_2m_max")]
    public List<double?> MaxTemperatures { get; set; } = new();

    [JsonPropertyName("temperature_2m_min")]
    public List<double?> MinTemperatures { get; set; } = new();

    [JsonPropertyName("precipitation_sum")]
    public List<double?> PrecipitationSums { get; set; } = new();
}