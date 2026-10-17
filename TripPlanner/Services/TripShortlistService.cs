using TripPlanner.Models;

namespace TripPlanner.Services;

// Keeps the shortlist in memory the doesn't need a database.
// It's registered as Scoped, so each user has their own list while the app is open.
public class TripShortlistService : ITripShortlistService
{
    private readonly List<ShortlistItem> _items = new();

    public bool Add(ShortlistItem item)
    {
        // Don't add the same place twice
        if (Contains(item.Place.Id))
        {
            return false;
        }

        _items.Add(item);
        return true;
    }

    public bool Remove(int placeId)
    {
        // RemoveAll returns how many it removed
        return _items.RemoveAll(i => i.Place.Id == placeId) > 0;
    }

    public bool Contains(int placeId)
    {
        return _items.Any(i => i.Place.Id == placeId);
    }

    public List<ShortlistItem> GetAll(ShortlistSort sortBy = ShortlistSort.Name)
    {
        // Sort a copy so the original list isn't changed
        var copy = new List<ShortlistItem>(_items);

        switch (sortBy)
        {
            case ShortlistSort.Warmest:
                // b compared to a, so the biggest number comes first
                copy.Sort((a, b) => b.AverageMaxTemperature.CompareTo(a.AverageMaxTemperature));
                break;

            case ShortlistSort.Driest:
                // the smallest rainfall comes first
                copy.Sort((a, b) => a.TotalRainMm.CompareTo(b.TotalRainMm));
                break;

            default:
                // uses CompareTo from ShortlistItem ,A to Z
                copy.Sort();
                break;
        }

        return copy;
    }
}
