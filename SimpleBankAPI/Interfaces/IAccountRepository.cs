using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Interfaces;

public interface IAccountRepository
{
    ValueTask<Account?> Get(Guid id);
    void Add(Account account);
    void Update(Account account, decimal amount);
}