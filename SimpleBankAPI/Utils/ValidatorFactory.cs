using System.Collections.Immutable;
using IValidator = SimpleBankAPI.Interfaces.IValidator;

namespace SimpleBankAPI.Utils;

public class ValidatorFactory
{
    private readonly IReadOnlyDictionary<string, IValidator> _validators;

    public ValidatorFactory()
    {
        var validatorType = typeof(IValidator);
        _validators = validatorType.Assembly.ExportedTypes
            .Where(x => validatorType.IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
            .Select(x=>Activator.CreateInstance(x))
            .Cast<IValidator>()
            .ToImmutableDictionary(x => x.GetType().Name, x => x);
    }

    public IValidator GetValidator(string validatorType)
    {
        var validator = _validators!.GetValueOrDefault(validatorType);
        return validator ?? throw new Exception("Could not find validator");
    }
}