namespace TripPlanner.Models;

// One place that the user saved to their shortlist, plus a few numbers from its forecast.
// It implements IComparable so a List of these can be sorted with list.Sort()
public class ShortlistItem : IComparable<ShortlistItem>
{
    public Destination Place { get; set; } = new();
    public double AverageMaxTemperature { get; set; }
    public double TotalRainMm { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.Now;

    // The default way to sort is A to Z by the place name
    public int CompareTo(ShortlistItem? other)
    {
        if (other is null)
        {
            return 1;
        }

        return string.Compare(Place.Name, other.Place.Name, StringComparison.OrdinalIgnoreCase);
    }
}