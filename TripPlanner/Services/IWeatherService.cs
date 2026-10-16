using TripPlanner.Models;

namespace TripPlanner.Services;

// Interface for getting a forecast.
// Same idea as IDestinationService, so I can use a fake version in unit tests.
public interface IWeatherService
{
    Task<List<DayForecast>> GetForecastAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}