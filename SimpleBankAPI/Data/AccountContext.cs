using Microsoft.EntityFrameworkCore;
using Account = SimpleBankAPI.Models.Entities.Account;
using ISavableCollection = SimpleBankAPI.Interfaces.ISavableCollection;

namespace SimpleBankAPI.Data;

public class AccountContext : DbContext, ISavableCollection
{
    public DbSet<Account> Accounts { private get; init; }
    
    public AccountContext(DbContextOptions<AccountContext> options) : base(options) {}

    public void Add(Account account) => Accounts.Add(account);

    public ValueTask<Account?> FindAsync(Guid id) => Accounts.FindAsync(id);
}