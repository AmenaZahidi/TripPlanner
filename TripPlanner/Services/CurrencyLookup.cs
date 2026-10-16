
namespace TripPlanner.Services;

// The geocoding API gives me a country code e.g. JP but the exchange rate API needs a
// currency code e.g.JPY, This is a lookup from one to the other.
// I only included currencies that Frankfurter supports.
public static class CurrencyLookup
{
    // Dictionary with the country code as the key StringComparer makes it ignore upper/lower case
    private static readonly Dictionary<string, string> CurrencyByCountry = new(StringComparer.OrdinalIgnoreCase)
    {
        // Euro countries
        ["IE"] = "EUR",
        ["FR"] = "EUR",
        ["DE"] = "EUR",
        ["ES"] = "EUR",
        ["IT"] = "EUR",
        ["PT"] = "EUR",
        ["NL"] = "EUR",
        ["BE"] = "EUR",
        ["AT"] = "EUR",
        ["FI"] = "EUR",
        ["GR"] = "EUR",
        ["LU"] = "EUR",
        ["MT"] = "EUR",
        ["CY"] = "EUR",
        ["SK"] = "EUR",
        ["SI"] = "EUR",
        ["EE"] = "EUR",
        ["LV"] = "EUR",
        ["LT"] = "EUR",
        ["HR"] = "EUR",
        ["BG"] = "EUR",

        // Other European countries
        ["GB"] = "GBP",
        ["CH"] = "CHF",
        ["SE"] = "SEK",
        ["NO"] = "NOK",
        ["DK"] = "DKK",
        ["PL"] = "PLN",
        ["CZ"] = "CZK",
        ["HU"] = "HUF",
        ["RO"] = "RON",
        ["IS"] = "ISK",
        ["TR"] = "TRY",

        // Rest of the world
        ["US"] = "USD",
        ["CA"] = "CAD",
        ["MX"] = "MXN",
        ["BR"] = "BRL",
        ["AU"] = "AUD",
        ["NZ"] = "NZD",
        ["JP"] = "JPY",
        ["CN"] = "CNY",
        ["HK"] = "HKD",
        ["KR"] = "KRW",
        ["IN"] = "INR",
        ["SG"] = "SGD",
        ["TH"] = "THB",
        ["MY"] = "MYR",
        ["ID"] = "IDR",
        ["PH"] = "PHP",
        ["IL"] = "ILS",
        ["ZA"] = "ZAR"
    };

    // Returns true and gives the currency code if I know the country, otherwise false
    public static bool TryGetCurrency(string? countryCode, out string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            currencyCode = string.Empty;
            return false;
        }

        return CurrencyByCountry.TryGetValue(countryCode, out currencyCode!);
    }
}