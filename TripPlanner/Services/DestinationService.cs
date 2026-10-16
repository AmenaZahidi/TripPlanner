using System.Net.Http.Json;
using System.Text.Json;
using TripPlanner.Models;

namespace TripPlanner.Services;

// This class searches for places (cities etc.) using the Open-Meteo geocoding API.
// It's free and doesn't need an API key.
// Source: https://open-meteo.com/en/docs/geocoding-api
public class DestinationService : IDestinationService
{
    private readonly HttpClient _http;

    public DestinationService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Destination>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        // If the search box is empty there's no point calling the API
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<Destination>();
        }

        // EscapeDataString makes the text safe to put in a URL (spaces, accents etc.)
        var url = $"v1/search?name={Uri.EscapeDataString(query.Trim())}&count=8&language=en&format=json";

        try
        {
            var response = await _http.GetFromJsonAsync<GeocodingResponse>(url, cancellationToken);

            // If nothing matches, the API doesn't send "results" at all, so it can be null.
            // I return an empty list in that case so the page doesn't crash.
            return response?.Results ?? new List<Destination>();
        }
        catch (HttpRequestException ex)
        {
            // Network problem or the API is down
            throw new DestinationServiceException(
                "Could not reach the destination search service. Please check your internet connection and try again.", ex);
        }
        catch (JsonException ex)
        {
            // The API sent back something we can't read
            throw new DestinationServiceException(
                "The destination search service sent back data we could not read. Please try again later.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // The request took longer than the timeout we set in Program.cs
            throw new DestinationServiceException(
                "The destination search took too long. Please try again.", ex);
        }
    }
}