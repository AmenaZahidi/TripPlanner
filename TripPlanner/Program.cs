using TripPlanner.Components;
using TripPlanner.Services;

namespace TripPlanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Place search (Open-Meteo geocoding API)
            builder.Services.AddHttpClient<IDestinationService, DestinationService>(client =>
            {
                client.BaseAddress = new Uri("https://geocoding-api.open-meteo.com/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            // Used by the weather and currency services so I don't keep calling the APIs
            builder.Services.AddMemoryCache();

            // 7 day forecast (Open-Meteo forecast API)
            builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
            {
                client.BaseAddress = new Uri("https://api.open-meteo.com/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            // Exchange rates (Frankfurter API)
            builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(client =>
            {
                client.BaseAddress = new Uri("https://api.frankfurter.dev/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });
            // Photo and description of the place Wikipedia REST API
            builder.Services.AddHttpClient<IWikipediaService, WikipediaService>(client =>
            {
                client.BaseAddress = new Uri("https://en.wikipedia.org/");
                client.Timeout = TimeSpan.FromSeconds(10);

                // Wikipedia asks every app to say who it is
                client.DefaultRequestHeaders.UserAgent.ParseAdd("TripPlanner/1.0 (https://github.com/AmenaZahidi/TripPlanner; student project)");
            });

            // The shortlist is kept in memory. Scoped means each user gets their own list.
            builder.Services.AddScoped<ITripShortlistService, TripShortlistService>();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}