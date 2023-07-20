namespace SimpleBankAPI.Interfaces;

public interface ISavableCollection<T>
{
    void Add(T account);
    ValueTask<T?> FindAsync(Guid id);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}