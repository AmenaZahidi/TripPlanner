using System.Text.Json.Serialization;

namespace TripPlanner.Models;

// The JSON reply from the Wikipedia "page summary" API.
// Example: https://en.wikipedia.org/api/rest_v1/page/summary/Tokyo
// The small classes below are the nested parts of that JSON.
public class WikipediaSummary
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("extract")]
    public string? Extract { get; set; }

    [JsonPropertyName("thumbnail")]
    public WikipediaImage? Thumbnail { get; set; }

    [JsonPropertyName("coordinates")]
    public WikipediaCoordinates? Coordinates { get; set; }

    [JsonPropertyName("content_urls")]
    public WikipediaContentUrls? ContentUrls { get; set; }
}

public class WikipediaImage
{
    [JsonPropertyName("source")]
    public string? Source { get; set; }
}

public class WikipediaCoordinates
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    public double Longitude { get; set; }
}

public class WikipediaContentUrls
{
    [JsonPropertyName("desktop")]
    public WikipediaPageLink? Desktop { get; set; }
}

public class WikipediaPageLink
{
    [JsonPropertyName("page")]
    public string? Page { get; set; }
}