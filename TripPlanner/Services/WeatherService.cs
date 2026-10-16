using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TripPlanner.Models;

namespace TripPlanner.Services;

// Gets a 7 day forecast from the Open-Meteo forecast API (free, no API key).
// Source: https://open-meteo.com/en/docs
public class WeatherService : IWeatherService
{
    // Keep results for 10 minutes so I don't call the API again for the same place
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public WeatherService(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<List<DayForecast>> GetForecastAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        // Round to 2 decimal places so nearly identical places share the same cache entry
        var lat = Math.Round(latitude, 2);
        var lon = Math.Round(longitude, 2);
        var cacheKey = string.Create(CultureInfo.InvariantCulture, $"forecast:{lat}:{lon}");

        // If I already have this forecast saved, use it instead of calling the API
        if (_cache.TryGetValue(cacheKey, out List<DayForecast>? cached) && cached is not null)
        {
            return cached;
        }

        // InvariantCulture so the decimal point is always "." (some PCs use "," which would break the URL)
        var url = string.Create(CultureInfo.InvariantCulture,
            $"v1/forecast?latitude={lat}&longitude={lon}&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum&timezone=auto&forecast_days=7");

        try
        {
            var response = await _http.GetFromJsonAsync<ForecastResponse>(url, cancellationToken);
            var forecast = ConvertToDays(response?.Daily);

            _cache.Set(cacheKey, forecast, CacheDuration);
            return forecast;
        }
        catch (HttpRequestException ex)
        {
            // Network problem or the API is down
            throw new WeatherServiceException(
                "Could not reach the weather service. Please check your internet connection and try again.", ex);
        }
        catch (JsonException ex)
        {
            // The API sent back something we can't read
            throw new WeatherServiceException(
                "The weather service sent back data we could not read. Please try again later.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // The request took longer than the timeout set in Program.cs
            throw new WeatherServiceException(
                "The weather service took too long to answer. Please try again.", ex);
        }
    }

    // The API sends separate lists (dates, temperatures etc.) where position 0 in each list is day 1.
    // This joins them into one DayForecast per day. It's public static so I can unit test it
    // without calling the real API.
    public static List<DayForecast> ConvertToDays(DailyForecastData? daily)
    {
        var days = new List<DayForecast>();

        if (daily is null)
        {
            return days;
        }

        for (var i = 0; i < daily.Dates.Count; i++)
        {
            // Skip any date we can't read instead of crashing
            if (!DateOnly.TryParseExact(daily.Dates[i], "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
            {
                continue;
            }

            days.Add(new DayForecast
            {
                Date = date,
                WeatherCode = GetValueAt(daily.WeatherCodes, i),
                MaxTemperature = GetValueAt(daily.MaxTemperatures, i),
                MinTemperature = GetValueAt(daily.MinTemperatures, i),
                PrecipitationMm = GetValueAt(daily.PrecipitationSums, i)
            });
        }

        return days;
    }

    // Gets item i from a list, or 0 if the list is too short or the value is null
    private static T GetValueAt<T>(List<T?> list, int index) where T : struct
    {
        return index < list.Count ? list[index] ?? default : default;
    }
}