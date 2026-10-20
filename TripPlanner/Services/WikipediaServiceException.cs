namespace TripPlanner.Services;

// My own exception for when getting the place info from Wikipedia goes wrong.
public class WikipediaServiceException : Exception
{
    public WikipediaServiceException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}