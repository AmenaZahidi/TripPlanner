using TripPlanner.Services;
using Xunit;

namespace TripPlanner.Tests;

// Tests for turning a country code into a currency code
public class CurrencyLookupTests
{
    [Theory]
    [InlineData("IE", "EUR")]
    [InlineData("JP", "JPY")]
    [InlineData("US", "USD")]
    [InlineData("GB", "GBP")]
    public void TryGetCurrency_KnownCountry_ReturnsCurrency(string country, string expected)
    {
        var found = CurrencyLookup.TryGetCurrency(country, out var currency);

        Assert.True(found);
        Assert.Equal(expected, currency);
    }

    [Fact]
    public void TryGetCurrency_LowerCaseCode_StillWorks()
    {
        var found = CurrencyLookup.TryGetCurrency("us", out var currency);

        Assert.True(found);
        Assert.Equal("USD", currency);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("XX")]
    public void TryGetCurrency_UnknownOrEmpty_ReturnsFalse(string? country)
    {
        var found = CurrencyLookup.TryGetCurrency(country, out _);

        Assert.False(found);
    }
}