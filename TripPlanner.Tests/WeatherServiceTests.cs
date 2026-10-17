using TripPlanner.Models;
using TripPlanner.Services;
using Xunit;

namespace TripPlanner.Tests;

// Tests for joining the API's separate lists into one DayForecast per day
public class WeatherServiceTests
{
    [Fact]
    public void ConvertToDays_TwoDays_JoinsTheListsCorrectly()
    {
        var data = new DailyForecastData
        {
            Dates = new List<string> { "2026-10-16", "2026-10-17" },
            WeatherCodes = new List<int?> { 0, 61 },
            MaxTemperatures = new List<double?> { 20.5, 18.0 },
            MinTemperatures = new List<double?> { 10.0, 9.5 },
            PrecipitationSums = new List<double?> { 0.0, 4.2 }
        };

        var days = WeatherService.ConvertToDays(data);

        Assert.Equal(2, days.Count);
        Assert.Equal(new DateOnly(2026, 10, 16), days[0].Date);
        Assert.Equal(20.5, days[0].MaxTemperature);
        Assert.Equal(61, days[1].WeatherCode);
        Assert.Equal(4.2, days[1].PrecipitationMm);
    }

    [Fact]
    public void ConvertToDays_NullData_ReturnsEmptyList()
    {
        var days = WeatherService.ConvertToDays(null);

        Assert.Empty(days);
    }
}