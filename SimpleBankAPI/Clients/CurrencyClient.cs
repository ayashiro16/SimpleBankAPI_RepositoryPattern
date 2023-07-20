using Newtonsoft.Json;
using ICurrencyRate = SimpleBankAPI.Interfaces.ICurrencyRate;

namespace SimpleBankAPI.Clients;

public class CurrencyClient: ICurrencyRate
{
    private const string _apiBaseUrl = "https://api.freecurrencyapi.com";
    private const string _apiKey = "fca_live_05342JeoKSmZWQYxyZpESGfZess61hoVb8qTguW5";

    private static readonly HttpClient _currencyClient = _currencyClient = new()
    {
        BaseAddress = new Uri($"{_apiBaseUrl}/v1/latest?apikey={_apiKey}")
    };

    public async Task<Dictionary<string, decimal>?> GetConversionRates(string? currencyCode)
    {
        var address = new UriBuilder(_currencyClient.BaseAddress);
        address.Query += $"&currencies={currencyCode}";
        var response = await _currencyClient.GetAsync(address.ToString());
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(responseBody);
        var rates = data?.GetValueOrDefault("data");

        return rates;
    }
}