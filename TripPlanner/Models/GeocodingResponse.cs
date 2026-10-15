using System.Text.Json.Serialization;

namespace TripPlanner.Models;

/// <summary>
/// Wraps the JSON returned by the Open-Meteo geocoding search.
/// Results is null when no place matches the search.
/// </summary>
public class GeocodingResponse
{
    [JsonPropertyName("results")]
    public List<Destination>? Results { get; set; }
}