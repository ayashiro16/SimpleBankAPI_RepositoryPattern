using SimpleBankAPI.Interfaces;
using SimpleBankAPI.Models.Responses;
using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Services;

public class AccountServices: IAccountServices
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrencyRate _currencyRate;
    private readonly IFactory<IValidator?> _validators;
    private const string Username = "Username";
    private const string Amount = "Amount";
    private const string CurrencyCode = "CurrencyCode";
    private const string SufficientFunds = "SufficientFunds";

    public AccountServices(IAccountRepository accountRepository, ICurrencyRate currencyRate, IFactory<IValidator?> validators)
    {
        _accountRepository = accountRepository;
        _currencyRate = currencyRate;
        _validators = validators;
    }
    
    /// <summary>
    /// Create and store an account with the provided name
    /// </summary>
    /// <param name="name">The account holder's name</param>
    /// <returns>The account details of our newly created account</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Account> CreateAccount(string name)
    {
        _validators[Username]?.Validate(name);
        var account = new Account()
        {
            Name = name, 
            Balance = 0, 
            Id = Guid.NewGuid()
        };
        _accountRepository.Add(account);

        return account;
    }

    /// <summary>
    /// Retrieves the account associated with the given ID
    /// </summary>
    /// <param name="id">The account Id</param>
    /// <returns>The account details</returns>
    public ValueTask<Account?> FindAccount(Guid id) => _accountRepository.Get(id);
    
    /// <summary>
    /// Deposits funds to an account
    /// </summary>
    /// <param name="id">The account ID</param>
    /// <param name="amount">The amount to be deposited</param>
    /// <returns>The account details following the deposit</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<Account?> DepositFunds(Guid id, decimal amount)
    {
        _validators[Amount]?.Validate(amount);
        var account = await _accountRepository.Get(id);
        if (account is null)
        {
            return account;
        }
        _accountRepository.Update(account, amount);
        
        return account;
    }

    /// <summary>
    /// Withdraws funds from an account
    /// </summary>
    /// <param name="id">The account ID</param>
    /// <param name="amount">The amount to be withdrawn</param>
    /// <returns>The account details following the withdraw</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Account?> WithdrawFunds(Guid id, decimal amount)
    {
        _validators[Amount]?.Validate(amount);
        var account = await _accountRepository.Get(id);
        if (account is null)
        {
            return account;
        }
        _validators[SufficientFunds]?.Validate((account.Balance, amount));
        _accountRepository.Update(account, amount * -1);
        
        return account;
    }

    /// <summary>
    /// Transfers funds from sender to recipient
    /// </summary>
    /// <param name="senderId">The account ID of the sender</param>
    /// <param name="recipientId">The account ID of the recipient</param>
    /// <param name="amount">The amount to be transferred</param>
    /// <returns>The account details of both the sender and the recipient following the transfer</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Transfer> TransferFunds(Guid senderId, Guid recipientId, decimal amount)
    {
        _validators[Amount]?.Validate(amount);
        var sender = await _accountRepository.Get(senderId);
        var recipient = await _accountRepository.Get(recipientId);
        if (sender is null || recipient is null)
        {
            return new Transfer(sender, recipient);
        }
        _validators[SufficientFunds]?.Validate((sender.Balance, amount));
        _accountRepository.Update(sender, amount * -1);
        _accountRepository.Update(recipient, amount);

        return new Transfer(sender, recipient);
    }

    public async Task<IEnumerable<ConvertCurrency>> GetConvertedCurrency(Guid id, string? currencies)
    {
        var account = await _accountRepository.Get(id);
        if (account is null)
        {
            throw new EntryPointNotFoundException("Could not find account associated with given ID");
        }
        if (!string.IsNullOrEmpty(currencies))
        {
            currencies = currencies.Replace(" ", string.Empty).ToUpper();
        }
        _validators[CurrencyCode]?.Validate(currencies);
        var rates = await _currencyRate.GetConversionRates(currencies?.Trim());
        if (rates.Count == 0)
        {
            throw new HttpRequestException("Could not retrieve currency rate data");
        }
        var balance = account.Balance;
        return rates.Select(rate => new ConvertCurrency(rate.Key, balance * (decimal)rate.Value));
    }
}