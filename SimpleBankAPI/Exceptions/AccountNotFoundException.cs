namespace SimpleBankAPI.Exceptions;

public class AccountNotFoundException : Exception
{
    public AccountNotFoundException()
        : base("Could not find an account with the provided ID")
    {
    }

    public AccountNotFoundException(string accountType)
        : base($"Could not find the {accountType} account")
    {
    }
}