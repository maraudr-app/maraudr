using System.Text.RegularExpressions;

namespace Maraudr.Associations.Domain.Siret;

public sealed class SiretNumber
{
    public string Value { get; } = null!;
    private SiretNumber() {}

    public SiretNumber(string value)
    {
        if (!Regex.IsMatch(value, @"^\d{14}$") || !PassesLuhnCheck(value))
            throw new ArgumentException("Invalid SIRET number", nameof(value));

        Value = value;
    }

    private static bool PassesLuhnCheck(string number)
    {
        var sum = 0;
        var alternate = false;

        for (var i = number.Length - 1; i >= 0; i--)
        {
            var digit = number[i] - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    public override bool Equals(object? obj) => obj is SiretNumber other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}