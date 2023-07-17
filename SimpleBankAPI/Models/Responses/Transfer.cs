using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Models.Responses;

public record Transfer(Account? Sender, Account? Recipient);