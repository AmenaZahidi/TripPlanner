using TripPlanner.Components;
using TripPlanner.Services;

namespace TripPlanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddHttpClient<IDestinationService, DestinationService>(client =>
            {
                client.BaseAddress = new Uri("https://geocoding-api.open-meteo.com/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });
            builder.Services.AddMemoryCache();

            builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
            {
                client.BaseAddress = new Uri("https://api.open-meteo.com/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(client =>
            {
                client.BaseAddress = new Uri("https://api.frankfurter.dev/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

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
