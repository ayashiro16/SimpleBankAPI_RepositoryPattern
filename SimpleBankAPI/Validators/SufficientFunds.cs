namespace SimpleBankAPI.Validators;

public class SufficientFunds : Interfaces.IValidator
{
    public bool Validate(object argument)
    {
        var args = ((decimal balance, decimal amount))argument;
        if (args.balance < args.amount)
        {
            throw new InvalidOperationException("Insufficient funds");
        }

        return true;
    }
}