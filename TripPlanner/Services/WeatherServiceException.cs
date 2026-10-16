namespace TripPlanner.Services;

// My own exception for when getting the weather goes wrong.
// The message is friendly so I can show it straight to the user.
public class WeatherServiceException : Exception
{
    public WeatherServiceException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}