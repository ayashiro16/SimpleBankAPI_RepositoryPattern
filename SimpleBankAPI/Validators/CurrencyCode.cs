namespace SimpleBankAPI.Validators;

public class CurrencyCode : Interfaces.IValidator
{
    public bool Validate(object? currencyCodes)
    {
        if (currencyCodes is null)
        {
            return true;
        }
        var codes = (string)currencyCodes;
        if (!codes.Trim().All(c=> char.IsUpper(c) || c == ','))
        {
            throw new ArgumentException("Must provide currency codes in all caps, separated by only commas if providing multiple codes");
        }
        
        return true;
    }
}