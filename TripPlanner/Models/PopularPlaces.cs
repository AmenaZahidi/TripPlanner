namespace TripPlanner.Models;

// The list of popular places. Both the Home page and the Destinations page use it.
public static class PopularPlaces
{
    public static readonly IReadOnlyList<PopularPlace> All = new List<PopularPlace>
    {
        new("Paris", "France", "🗼", "linear-gradient(135deg, #ff7e5f, #d6336c)"),
        new("Rome", "Italy", "🏛️", "linear-gradient(135deg, #f7971e, #e8590c)"),
        new("Tokyo", "Japan", "🗾", "linear-gradient(135deg, #7f5af0, #3b3b98)"),
        new("New York", "United States", "🗽", "linear-gradient(135deg, #1a5fd0, #0d2a5c)"),
        new("Barcelona", "Spain", "⛪", "linear-gradient(135deg, #e63946, #9d0208)"),
        new("Lisbon", "Portugal", "🚋", "linear-gradient(135deg, #00b4a6, #00798c)")
    };
}