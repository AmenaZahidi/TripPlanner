# TripPlanner

TripPlanner is my CA1 project: a Blazor web app written in C# on .NET 10. I picked a trip planner because it's something people actually do. You're trying to decide where to go, so you want to know what the weather will be like, what your money is worth there, and which place looks best once you put them side by side.

Name:Amena Zahidi
Student number :D00258912
Module: SOA

## What the app does

You search for a city on the Destinations page. Lots of cities share a name (there's a Dublin in Ireland and another in Texas), so each result shows its region and country and you can pick the right one. Click "View forecast" and you get the weather for the next 7 days: an icon, a short description, the high and low temperature and how much rain to expect.

Above the forecast there's a currency card. It shows what an amount in euros is worth in the local currency, and you can change the amount and the answer updates straight away without another API call.

If you like a place, you can add it to your shortlist. On the Shortlist page you can sort your saved places by name, by warmest or by driest, which makes it easy to compare them.

## The two web services

I used two different providers, and neither needs an API key.

Open-Meteo gives me two things: a geocoding API that finds places by name, and a forecast API that returns the 7 day weather. Frankfurter is a separate service that gives live exchange rates. I use it to convert euros into the currency of whichever country you picked.

The geocoding API gives back a country code like "JP", but Frankfurter wants a currency code like "JPY". So I wrote a small lookup class that turns one into the other. It only covers currencies Frankfurter supports, and if a country isn't in it the card says so instead of showing something wrong.

## Running it

Open `TripPlanner.slnx` in Visual Studio (you need .NET 10) and press F5. Go to Destinations, search for something like Tokyo and click "View forecast". To run the tests go to Test, then Run All Tests.

## How the code is laid out

The Models folder holds the classes that just store data, like Destination, DayForecast, ExchangeRate and ShortlistItem. The Services folder has the classes that talk to the APIs, plus an interface for each one and my own exception classes. The Components folder has the pages and the currency card. The tests live in their own project, TripPlanner.Tests.

Each service has an interface (IDestinationService, IWeatherService and so on). I did that mainly so I could test them with fakes instead of the real internet.

## C# things I used

I used List<T> for the search results, the forecast days and the shortlist, and a Dictionary for the country to currency lookup. For sorting, ShortlistItem implements IComparable so a list of them sorts A to Z by default, and the warmest and driest options sort with a comparison lambda. I also used an enum for the sort choices, DateOnly for dates, decimal for money, nullable types for values the API can leave out, a switch expression to turn a weather code into text and an icon, and async/await for every API call.

## Dealing with the API responses

Each API is called through a typed HttpClient that I set up in Program.cs with a 10 second timeout. The JSON is read straight into model classes with GetFromJsonAsync.

One thing that took me a minute to work out is that Open-Meteo sends the forecast as separate lists, one for dates, one for max temperatures, one for rain and so on. I wrote a method called ConvertToDays that lines them up and builds one DayForecast per day.

My lecturer said not to keep calling the API, so I added caching with IMemoryCache. A forecast is kept for 10 minutes and an exchange rate for 30. If you look at the same place again the app uses the saved copy instead of making another request.

## When things go wrong

Web services fail in all sorts of ways: the internet drops, the server is down, the reply isn't what you expected, or it just takes too long. Each service catches those problems and throws its own exception (DestinationServiceException, WeatherServiceException or CurrencyServiceException) with a message a normal person can understand. The pages catch that and show the message on screen, so the app doesn't crash, and a spinner shows while data is loading.

## Tests

I used xUnit. The tests cover the weather code lookup, the currency lookup, the code that joins the forecast lists, and the shortlist (adding, removing, blocking duplicates and the three sort orders).

I also tested DestinationService with a class I wrote called FakeHttpHandler. It pretends to be the internet, so I can check what happens with a good reply, no results, a blank search, a server error and invalid JSON without making a real request.

## Limitations

The search matches place names, not countries, so searching "Usa" finds a small town in Japan rather than the United States. The shortlist is kept in memory, so it's cleared if you refresh the page or restart the app, and I didn't add a database because the brief didn't ask for one. The currency lookup only covers about 60 countries. With more time I'd add a way to filter results by country and save the shortlist between visits.

## References

- Open-Meteo forecast API: https://open-meteo.com/en/docs
- Open-Meteo geocoding API: https://open-meteo.com/en/docs/geocoding-api
- Frankfurter exchange rates API: https://frankfurter.dev
- Microsoft Learn, Blazor: https://learn.microsoft.com/aspnet/core/blazor/
- Microsoft Learn, HttpClientFactory: https://learn.microsoft.com/dotnet/core/extensions/httpclient-factory
- Microsoft Learn, in-memory caching: https://learn.microsoft.com/aspnet/core/performance/caching/memory
- xUnit documentation: https://xunit.net/docs/getting-started/v2/getting-started
- Bootstrap 5, which comes with the Blazor template: https://getbootstrap.com


