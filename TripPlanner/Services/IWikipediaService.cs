using TripPlanner.Models;

namespace TripPlanner.Services;

// Interface for getting a photo and short description of a place.
public interface IWikipediaService
{
    // Returns null if  can't find a page that matches the place
    Task<PlaceInfo?> GetInfoAsync(Destination place, CancellationToken cancellationToken = default);
}