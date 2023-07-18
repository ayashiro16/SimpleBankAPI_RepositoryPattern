using Transfer = SimpleBankAPI.Models.Responses.Transfer;
using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Interfaces;

public interface IAccountServices
{
    Task<Account> CreateAccount(string name);
    ValueTask<Account?> FindAccount(Guid id);
    Task<Account?> DepositFunds(Guid id, decimal amount);
    Task<Account?> WithdrawFunds(Guid id, decimal amount);
    Task<Transfer> TransferFunds(Guid senderId, Guid recipientId, decimal amount);
}