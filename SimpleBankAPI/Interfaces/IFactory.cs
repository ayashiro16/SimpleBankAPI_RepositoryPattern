namespace SimpleBankAPI.Interfaces;

public interface IFactory<out T>
{
    T this[string key] { get; }
}