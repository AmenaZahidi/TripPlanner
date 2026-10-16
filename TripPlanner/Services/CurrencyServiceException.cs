namespace TripPlanner.Services;

// My own exception for when getting the exchange rate goes wrong.
// The message is friendly so I can show it straight to the user.
public class CurrencyServiceException : Exception
{
    public CurrencyServiceException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}