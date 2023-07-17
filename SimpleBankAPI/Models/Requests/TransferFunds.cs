namespace SimpleBankAPI.Models.Requests;

public class TransferFunds
{
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public decimal Amount { get; set; }
}