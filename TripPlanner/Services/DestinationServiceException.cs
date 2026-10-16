namespace TripPlanner.Services;

// My own exception for when the destination search goes wrong.
// The message is friendly so I can show it straight to the user on the page.
public class DestinationServiceException : Exception
{
    public DestinationServiceException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}