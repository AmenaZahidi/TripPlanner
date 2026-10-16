namespace TripPlanner.Services;

// Open-Meteo sends the weather as a number (a WMO weather code, e.g. 61 = rain).
// This turns the number into words and an emoji I can show on the page.
// Code list: https://open-meteo.com/en/docs (search for "WMO Weather interpretation codes")
public static class WeatherCodeHelper
{
    public static string GetDescription(int code) => code switch
    {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 or 48 => "Fog",
        >= 51 and <= 55 => "Drizzle",
        56 or 57 => "Freezing drizzle",
        >= 61 and <= 65 => "Rain",
        66 or 67 => "Freezing rain",
        >= 71 and <= 75 => "Snow",
        77 => "Snow grains",
        >= 80 and <= 82 => "Rain showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorm",
        96 or 99 => "Thunderstorm with hail",
        _ => "Unknown"
    };

    public static string GetIcon(int code) => code switch
    {
        0 => "☀️",
        1 => "🌤️",
        2 => "⛅",
        3 => "☁️",
        45 or 48 => "🌫️",
        >= 51 and <= 57 => "🌦️",
        >= 61 and <= 67 => "🌧️",
        >= 71 and <= 77 => "❄️",
        >= 80 and <= 82 => "🌧️",
        85 or 86 => "🌨️",
        >= 95 and <= 99 => "⛈️",
        _ => "❓"
    };
}