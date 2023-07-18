namespace SimpleBankAPI.Interfaces;

public interface IAccountRepository<T>
{
    ValueTask<T?> Get(Guid id);
    T Add(T account);
    T Update(T account, decimal amount);
}