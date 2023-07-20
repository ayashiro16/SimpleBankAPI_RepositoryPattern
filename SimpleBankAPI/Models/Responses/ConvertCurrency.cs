using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Models.Responses;

public record ConvertCurrency(string? CurrencyCode, decimal? ConvertedBalance);