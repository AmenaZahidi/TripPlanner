using TripPlanner.Models;

namespace TripPlanner.Services;

// The different ways the shortlist can be sorted
public enum ShortlistSort
{
    Name,
    Warmest,
    Driest
}

// Interface for the shortlist, so I can use it in unit tests
public interface ITripShortlistService
{
    // Returns false if the place is already on the list
    bool Add(ShortlistItem item);

    bool Remove(int placeId);

    bool Contains(int placeId);

    List<ShortlistItem> GetAll(ShortlistSort sortBy = ShortlistSort.Name);
}