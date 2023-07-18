using SimpleBankAPI.Interfaces;
using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Repositories;

public class AccountRepository : Interfaces.IAccountRepository<Account>
{
    private readonly ISavableCollection<Account> _context;

    public AccountRepository(ISavableCollection<Account> context)
    {
        _context = context;
    }
    
    public ValueTask<Account?> Get(Guid id)
    {
        return _context.FindAsync(id);
    }

    public Account Add(Account account)
    {
        _context.Add(account);
        _context.SaveChangesAsync();
        return account;
    }

    public Account Update(Account account, decimal amount)
    {
        account.Balance += amount;
        _context.SaveChangesAsync();
        return account;
    }
}