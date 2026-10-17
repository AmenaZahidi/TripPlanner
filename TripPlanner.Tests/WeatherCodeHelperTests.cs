using TripPlanner.Services;
using Xunit;

namespace TripPlanner.Tests;

// Tests for the weather code lookup turns a number like 0 into Clear sky
public class WeatherCodeHelperTests
{
    // Theory runs the same test once for each InlineData line
    [Theory]
    [InlineData(0, "Clear sky")]
    [InlineData(1, "Mainly clear")]
    [InlineData(2, "Partly cloudy")]
    [InlineData(3, "Overcast")]
    public void GetDescription_KnownCode_ReturnsText(int code, string expected)
    {
        Assert.Equal(expected, WeatherCodeHelper.GetDescription(code));
    }

    [Fact]
    public void GetDescription_UnknownCode_ReturnsUnknown()
    {
        Assert.Equal("Unknown", WeatherCodeHelper.GetDescription(999));
    }

    [Fact]
    public void GetIcon_UnknownCode_ReturnsQuestionMark()
    {
        Assert.Equal("❓", WeatherCodeHelper.GetIcon(999));
    }
}