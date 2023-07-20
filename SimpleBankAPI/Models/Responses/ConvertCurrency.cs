namespace SimpleBankAPI.Models.Responses;

public record ConvertCurrency(string? CurrencyCode, decimal? ConvertedBalance);