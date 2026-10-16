using System.Text.Json.Serialization;

namespace TripPlanner.Models;

// The JSON reply from the Frankfurter exchange rate API.
// Example: {amount:1.0, base:,EUR,date":2026-10-14,rates:USD:1.1195}
// Source: https://frankfurter.dev
public class ExchangeRateResponse
{
    [JsonPropertyName("base")]
    public string Base { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    // Dictionary, the key is the currency code e.g. USD,and the value is the rate
    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Rates { get; set; } = new();
}
