using TripPlanner.Models;
using TripPlanner.Services;
using Xunit;

namespace TripPlanner.Tests;

// Tests for the shortlist (adding, removing and sorting
public class TripShortlistServiceTests
{
    // Small helper so I don't  have repeat the same code in every test
    private static ShortlistItem MakeItem(int id, string name, double temp, double rain)
    {
        return new ShortlistItem
        {
            Place = new Destination { Id = id, Name = name },
            AverageMaxTemperature = temp,
            TotalRainMm = rain
        };
    }

    [Fact]
    public void Add_NewPlace_ReturnsTrue()
    {
        var service = new TripShortlistService();

        Assert.True(service.Add(MakeItem(1, "Dublin", 12, 5)));
        Assert.True(service.Contains(1));
    }

    [Fact]
    public void Add_SamePlaceTwice_ReturnsFalseAndKeepsOne()
    {
        var service = new TripShortlistService();
        service.Add(MakeItem(1, "Dublin", 12, 5));

        var addedAgain = service.Add(MakeItem(1, "Dublin", 12, 5));

        Assert.False(addedAgain);
        Assert.Single(service.GetAll());
    }

    [Fact]
    public void Remove_ExistingPlace_RemovesIt()
    {
        var service = new TripShortlistService();
        service.Add(MakeItem(1, "Dublin", 12, 5));

        Assert.True(service.Remove(1));
        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void Remove_PlaceNotOnList_ReturnsFalse()
    {
        var service = new TripShortlistService();

        Assert.False(service.Remove(99));
    }

    [Fact]
    public void GetAll_SortByName_IsAToZ()
    {
        var service = new TripShortlistService();
        service.Add(MakeItem(1, "Tokyo", 20, 1));
        service.Add(MakeItem(2, "Athens", 25, 0));
        service.Add(MakeItem(3, "Madrid", 22, 2));

        var names = service.GetAll(ShortlistSort.Name).Select(i => i.Place.Name).ToList();

        Assert.Equal(new List<string> { "Athens", "Madrid", "Tokyo" }, names);
    }

    [Fact]
    public void GetAll_SortByWarmest_HottestFirst()
    {
        var service = new TripShortlistService();
        service.Add(MakeItem(1, "Cold", 5, 1));
        service.Add(MakeItem(2, "Hot", 30, 1));
        service.Add(MakeItem(3, "Mild", 15, 1));

        var names = service.GetAll(ShortlistSort.Warmest).Select(i => i.Place.Name).ToList();

        Assert.Equal(new List<string> { "Hot", "Mild", "Cold" }, names);
    }

    [Fact]
    public void GetAll_SortByDriest_LeastRainFirst()
    {
        var service = new TripShortlistService();
        service.Add(MakeItem(1, "Wet", 10, 30));
        service.Add(MakeItem(2, "Dry", 10, 0));
        service.Add(MakeItem(3, "Damp", 10, 8));

        var names = service.GetAll(ShortlistSort.Driest).Select(i => i.Place.Name).ToList();

        Assert.Equal(new List<string> { "Dry", "Damp", "Wet" }, names);
    }
}