using TripPlanner.Models;

namespace TripPlanner.Services;

// Interface for searching places.
// I'm using an interface so in my unit tests I can use a fake version
// instead of calling the real API every time.
public interface IDestinationService
{
    Task<List<Destination>> SearchAsync(string query, CancellationToken cancellationToken = default);
}