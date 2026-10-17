namespace TripPlanner.Tests;

// A fake "internet" for my tests. Instead of calling the real API,
// HttpClient asks this class, and it gives back whatever I tell it to.
public class FakeHttpHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

    // Counts how many times the API was called to check caching works
    public int CallCount { get; private set; }

    public FakeHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        _respond = respond;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CallCount++;
        return Task.FromResult(_respond(request));
    }
}