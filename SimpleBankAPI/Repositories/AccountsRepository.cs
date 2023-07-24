using SimpleBankAPI.Interfaces;
using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Repositories;

public class AccountsRepository : IAccountsRepository
{
    private readonly ISavableCollection<Account> _context;

    public AccountsRepository(ISavableCollection<Account> context)
    {
        _context = context;
    }
    
    public ValueTask<Account?> Get(Guid id)
    {
        return _context.FindAsync(id);
    }

    public void Add(Account account)
    {
        _context.Add(account);
        _context.SaveChangesAsync();
    }

    public void Update(Account account, decimal amount)
    {
        account.Balance += amount;
        _context.SaveChangesAsync();
    }
}