using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TripPlanner.Models;

namespace TripPlanner.Services;

// Gets exchange rates from the Frankfurter API (free, no API key).
// Source: https://frankfurter.dev
public class CurrencyService : ICurrencyService
{
    // Rates only change once a day, so keeping them for 30 minutes is fine
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public CurrencyService(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<ExchangeRate?> GetRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken = default)
    {
        var from = fromCurrency.ToUpperInvariant();
        var to = toCurrency.ToUpperInvariant();

        // Same currency, so the rate is just 1 and there's no need to call the API
        if (from == to)
        {
            return new ExchangeRate { FromCurrency = from, ToCurrency = to, Rate = 1m, Date = "today" };
        }

        // If I already have this rate saved, use it instead of calling the API again
        var cacheKey = $"rate:{from}:{to}";
        if (_cache.TryGetValue(cacheKey, out ExchangeRate? cached) && cached is not null)
        {
            return cached;
        }

        var url = $"v1/latest?base={Uri.EscapeDataString(from)}&symbols={Uri.EscapeDataString(to)}";

        try
        {
            var response = await _http.GetFromJsonAsync<ExchangeRateResponse>(url, cancellationToken);

            // No rate for that currency, so return null and let the page say so
            if (response is null || !response.Rates.TryGetValue(to, out var rate))
            {
                return null;
            }

            var result = new ExchangeRate
            {
                FromCurrency = from,
                ToCurrency = to,
                Rate = rate,
                Date = response.Date
            };

            _cache.Set(cacheKey, result, CacheDuration);
            return result;
        }
        catch (HttpRequestException ex)
        {
            // Network problem, the API is down, or it doesn't know that currency
            throw new CurrencyServiceException(
                "Could not get the exchange rate. Please check your internet connection and try again.", ex);
        }
        catch (JsonException ex)
        {
            // The API sent back something we can't read
            throw new CurrencyServiceException(
                "The exchange rate service sent back data we could not read. Please try again later.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // The request took longer than the timeout set in Program.cs
            throw new CurrencyServiceException(
                "The exchange rate service took too long to answer. Please try again.", ex);
        }
    }
}