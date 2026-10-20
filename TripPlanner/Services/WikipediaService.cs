using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TripPlanner.Models;

namespace TripPlanner.Services;

// Gets a photo and a short description of a place from the Wikipedia REST API free, no key.
// Docs: https://en.wikipedia.org/api/rest_v1/
public class WikipediaService : IWikipediaService
{
    // A Wikipedia page only counts if its coordinates are this close to my place
    private const double MaxDistanceKm = 50;

    // Descriptions don't really change, so I keep them for a day
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public WikipediaService(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<PlaceInfo?> GetInfoAsync(Destination place, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"wiki:{place.Id}";

        // GetOrCreateAsync uses the saved copy if there is one, and only calls Wikipedia if not.
        // It also saves null, so a place with no page isn't searched for again and again.
        return await _cache.GetOrCreateAsync<PlaceInfo?>(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await FindInfoAsync(place, cancellationToken);
        });
    }

    private async Task<PlaceInfo?> FindInfoAsync(Destination place, CancellationToken cancellationToken)
    {
        try
        {
            // Try the most exact title first, then less exact ones
            foreach (var title in BuildCandidateTitles(place))
            {
                var summary = await GetSummaryAsync(title, cancellationToken);

                if (summary is null || !IsSamePlace(summary, place))
                {
                    continue;
                }

                return new PlaceInfo
                {
                    Title = summary.Title ?? place.Name,
                    Description = summary.Description,
                    Summary = summary.Extract,
                    ImageUrl = summary.Thumbnail?.Source,
                    PageUrl = summary.ContentUrls?.Desktop?.Page
                };
            }

            // Nothing matched
            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new WikipediaServiceException(
                "Could not load the place information. Please try again later.", ex);
        }
        catch (JsonException ex)
        {
            throw new WikipediaServiceException(
                "The place information service sent back data we could not read.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new WikipediaServiceException(
                "The place information service took too long to answer.", ex);
        }
    }

    // Calls the API for one page title. Returns null if there is no page with that title (404).
    private async Task<WikipediaSummary?> GetSummaryAsync(string title, CancellationToken cancellationToken)
    {
        var url = $"api/rest_v1/page/summary/{Uri.EscapeDataString(title.Replace(' ', '_'))}";

        using var response = await _http.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<WikipediaSummary>(cancellationToken: cancellationToken);
    }

    // Many places share a name, so I try "Name, Region", then "Name, Country", then just "Name".
    public static List<string> BuildCandidateTitles(Destination place)
    {
        var titles = new List<string>();

        if (!string.IsNullOrWhiteSpace(place.Region))
        {
            titles.Add($"{place.Name}, {place.Region}");
        }

        if (!string.IsNullOrWhiteSpace(place.Country))
        {
            titles.Add($"{place.Name}, {place.Country}");
        }

        titles.Add(place.Name);

        return titles;
    }

    // Checks the Wikipedia page is really about my place, so I never show the wrong city's photo.
    // e.g. "Tokyo" in Papua New Guinea must not get a photo of Tokyo in Japan.
    public static bool IsSamePlace(WikipediaSummary summary, Destination place)
    {
        // A disambiguation page is just a list of other pages
        if (summary.Type == "disambiguation")
        {
            return false;
        }

        // No coordinates, so I can't be sure it is the same place
        if (summary.Coordinates is null)
        {
            return false;
        }

        var distance = GeoHelper.DistanceKm(
            place.Latitude, place.Longitude,
            summary.Coordinates.Latitude, summary.Coordinates.Longitude);

        return distance <= MaxDistanceKm;
    }
}