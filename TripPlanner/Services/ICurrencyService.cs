using TripPlanner.Models;

namespace TripPlanner.Services;

// Interface for getting exchange rates, so I can use a fake version in unit tests.
public interface ICurrencyService
{
    // Returns null if there is no rate available for that currency
    Task<ExchangeRate?> GetRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken = default);
}