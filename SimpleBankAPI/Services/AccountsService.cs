using SimpleBankAPI.Exceptions;
using SimpleBankAPI.Interfaces;
using SimpleBankAPI.Models.Responses;
using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Services;

public class AccountsService: IAccountsService
{
    private readonly IAccountsRepository _accountsRepository;
    private readonly ICurrencyRate _currencyRate;
    private readonly IFactory<IValidator?> _validators;
    private const string Username = "Username";
    private const string Amount = "Amount";
    private const string CurrencyCode = "CurrencyCode";
    private const string SufficientFunds = "SufficientFunds";

    public AccountsService(IAccountsRepository accountsRepository, ICurrencyRate currencyRate, IFactory<IValidator?> validators)
    {
        _accountsRepository = accountsRepository;
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
        _accountsRepository.Add(account);

        return account;
    }

    /// <summary>
    /// Retrieves the account associated with the given ID
    /// </summary>
    /// <param name="id">The account Id</param>
    /// <returns>The account details</returns>
    public async ValueTask<Account?> FindAccount(Guid id)
    {
        var account = await _accountsRepository.Get(id);
        if (account is null)
        {
            throw new AccountNotFoundException();
        }

        return account;
    }
    
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
        var account = await _accountsRepository.Get(id);
        if (account is null)
        {
            throw new AccountNotFoundException();
        }
        _accountsRepository.Update(account, amount);
        
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
        var account = await _accountsRepository.Get(id);
        if (account is null)
        {
            throw new AccountNotFoundException();
        }
        _validators[SufficientFunds]?.Validate((account.Balance, amount));
        _accountsRepository.Update(account, amount * -1);
        
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
        var sender = await _accountsRepository.Get(senderId);
        var recipient = await _accountsRepository.Get(recipientId);
        if (sender is null || recipient is null)
        {
            return new Transfer(sender, recipient) switch
            {
                { Sender: null, Recipient: null } => throw new AccountNotFoundException("sender or recipient"),
                { Sender: null, Recipient: not null } => throw new AccountNotFoundException("sender"),
                { Sender: not null, Recipient: null } => throw new AccountNotFoundException("recipient"),
            };
        }
        _validators[SufficientFunds]?.Validate((sender.Balance, amount));
        _accountsRepository.Update(sender, amount * -1);
        _accountsRepository.Update(recipient, amount);

        return new Transfer(sender, recipient);
    }

    public async Task<IEnumerable<ConvertCurrency>> GetConvertedCurrency(Guid id, string currencies)
    {
        var account = await _accountsRepository.Get(id);
        if (account is null)
        {
            throw new NullAccountException();
        }
        currencies = currencies.Replace(" ", string.Empty).ToUpper();
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