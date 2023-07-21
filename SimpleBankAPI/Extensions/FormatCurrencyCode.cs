namespace SimpleBankAPI.Extensions;

public static class FormatCurrencyCode
{
    public static string Format(this string codes)
    {
        return string.IsNullOrEmpty(codes) ? 
            codes.Replace(" ", string.Empty).ToUpper()
            : string.Empty;
    }
}