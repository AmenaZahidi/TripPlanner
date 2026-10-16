namespace TripPlanner.Models;

// One day of the forecast in a simple form that's easy to show on the page.
// I build a list of these from the lists in DailyForecastData.
public class DayForecast
{
    public DateOnly Date { get; set; }
    public int WeatherCode { get; set; }
    public double MaxTemperature { get; set; }
    public double MinTemperature { get; set; }
    public double PrecipitationMm { get; set; }
}