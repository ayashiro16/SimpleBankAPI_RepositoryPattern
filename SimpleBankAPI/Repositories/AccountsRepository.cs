using AccountContext = SimpleBankAPI.Data.AccountContext;
using IAccountsRepository = SimpleBankAPI.Interfaces.IAccountsRepository;
using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Repositories;

public class AccountsRepository : IAccountsRepository
{
    private readonly AccountContext _context;

    public AccountsRepository(AccountContext context)
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