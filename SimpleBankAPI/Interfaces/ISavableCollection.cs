using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Interfaces;

public interface ISavableCollection
{
    void Add(Account account);
    ValueTask<Account?> FindAsync(Guid id);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}