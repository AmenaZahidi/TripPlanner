namespace TripPlanner.Models;

// The simple version of the Wikipedia reply that my page shows.
public class PlaceInfo
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public string? ImageUrl { get; set; }
    public string? PageUrl { get; set; }
}