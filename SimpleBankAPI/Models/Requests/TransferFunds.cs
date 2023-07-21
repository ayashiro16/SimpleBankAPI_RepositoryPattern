namespace SimpleBankAPI.Models.Requests;

public record TransferFunds(Guid SenderId, Guid RecipientId, decimal Amount);