using Microsoft.EntityFrameworkCore;
using SimpleBankAPI.Interfaces;
using SimpleBankAPI.Models.Responses;
using Account = SimpleBankAPI.Models.Entities.Account;

namespace SimpleBankAPI.Services;

public class AccountServices: IAccountServices
{
    private readonly IAccountRepository _accountRepository;
    
    public AccountServices(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    
    /// <summary>
    /// Create and store an account with the provided name
    /// </summary>
    /// <param name="name">The account holder's name</param>
    /// <returns>The account details of our newly created account</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Account> CreateAccount(string name)
    {
        ValidateName(name);
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
        ValidatePositiveAmount(amount);
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
        ValidatePositiveAmount(amount);
        var account = await _accountRepository.Get(id);
        if (account is null)
        {
            return account;
        }
        ValidateSufficientFunds(account.Balance, amount);
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
        ValidatePositiveAmount(amount);
        var sender = await _accountRepository.Get(senderId);
        var recipient = await _accountRepository.Get(recipientId);
        if (sender is null || recipient is null)
        {
            return new Transfer(sender, recipient);
        }
        ValidateSufficientFunds(sender.Balance, amount);
        _accountRepository.Update(sender, amount * -1);
        _accountRepository.Update(recipient, amount);

        return new Transfer(sender, recipient);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name field cannot be empty or white space");
        }
        if (!name.All(x => char.IsWhiteSpace(x) || char.IsLetter(x)))
        {
            throw new ArgumentException("Name cannot contain special characters or numbers");
        }
    }
    
    private static void ValidatePositiveAmount(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Cannot give a negative amount");
        }
    }

    private static void ValidateSufficientFunds(decimal balance, decimal amount)
    {
        if (balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds");
        }
    }
}