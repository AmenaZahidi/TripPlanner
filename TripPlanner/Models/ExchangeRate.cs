namespace TripPlanner.Models;

// A simple version of the exchange rate that's easy to show on the page.
public class ExchangeRate
{
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string Date { get; set; } = string.Empty;
}