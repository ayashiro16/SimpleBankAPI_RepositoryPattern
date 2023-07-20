namespace SimpleBankAPI.Validators;

public class CurrencyCode : Interfaces.IValidator
{
    public bool Validate(object currencyCodes)
    {
        if (!((string)currencyCodes).All(c=> char.IsUpper(c) || c == ','))
        {
            throw new ArgumentException("Must provide currency codes in all caps, separated by only commas if providing multiple codes");
        }
        
        return true;
    }
}