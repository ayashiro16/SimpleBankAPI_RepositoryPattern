namespace SimpleBankAPI.Interfaces;

public interface ICurrencyRate
{
    Task<Dictionary<string, decimal>> GetConversionRates(string? currencyCode);
}