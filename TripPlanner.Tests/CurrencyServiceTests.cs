using System.Net;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using TripPlanner.Services;
using Xunit;

namespace TripPlanner.Tests;

// Tests for CurrencyService using a fake API (no real internet needed)
public class CurrencyServiceTests
{
    // Helper: builds a CurrencyService that uses my fake handler
    private static CurrencyService MakeService(FakeHttpHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.frankfurter.dev/") };
        var cache = new MemoryCache(new MemoryCacheOptions());
        return new CurrencyService(client, cache);
    }

    private static HttpResponseMessage JsonResponse(string json, HttpStatusCode status = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    [Fact]
    public async Task GetRateAsync_GoodResponse_ReturnsRate()
    {
        var handler = new FakeHttpHandler(_ => JsonResponse(
            "{\"amount\":1.0,\"base\":\"EUR\",\"date\":\"2026-10-16\",\"rates\":{\"USD\":1.1}}"));
        var service = MakeService(handler);

        var result = await service.GetRateAsync("EUR", "USD");

        Assert.NotNull(result);
        Assert.Equal(1.1m, result.Rate);
        Assert.Equal("USD", result.ToCurrency);
    }

    [Fact]
    public async Task GetRateAsync_CalledTwice_OnlyCallsApiOnce()
    {
        var handler = new FakeHttpHandler(_ => JsonResponse(
            "{\"amount\":1.0,\"base\":\"EUR\",\"date\":\"2026-10-16\",\"rates\":{\"USD\":1.1}}"));
        var service = MakeService(handler);

        await service.GetRateAsync("EUR", "USD");
        await service.GetRateAsync("EUR", "USD");

        // The second call should come from the cache
        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task GetRateAsync_SameCurrency_ReturnsOneWithoutCallingApi()
    {
        var handler = new FakeHttpHandler(_ => JsonResponse("{}"));
        var service = MakeService(handler);

        var result = await service.GetRateAsync("EUR", "EUR");

        Assert.NotNull(result);
        Assert.Equal(1m, result.Rate);
        Assert.Equal(0, handler.CallCount);
    }

    [Fact]
    public async Task GetRateAsync_ServerError_ThrowsFriendlyException()
    {
        var handler = new FakeHttpHandler(_ => JsonResponse("{}", HttpStatusCode.InternalServerError));
        var service = MakeService(handler);

        await Assert.ThrowsAsync<CurrencyServiceException>(() => service.GetRateAsync("EUR", "USD"));
    }

    [Fact]
    public async Task GetRateAsync_BadJson_ThrowsFriendlyException()
    {
        var handler = new FakeHttpHandler(_ => JsonResponse("this is not json"));
        var service = MakeService(handler);

        await Assert.ThrowsAsync<CurrencyServiceException>(() => service.GetRateAsync("EUR", "USD"));
    }

    [Fact]
    public async Task GetRateAsync_NetworkDown_ThrowsFriendlyException()
    {
        // The fake handler throws the same error a real network failure would
        var handler = new FakeHttpHandler(_ => throw new HttpRequestException("no internet"));
        var service = MakeService(handler);

        await Assert.ThrowsAsync<CurrencyServiceException>(() => service.GetRateAsync("EUR", "USD"));
    }
}