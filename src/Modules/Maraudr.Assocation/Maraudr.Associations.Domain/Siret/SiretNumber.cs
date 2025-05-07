using System.Text.RegularExpressions;

namespace Maraudr.Associations.Domain.Siret;

public readonly struct SiretNumber
{
    public string Value { get; }

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

    private bool Equals(SiretNumber other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        return obj.GetType() == GetType() && Equals((SiretNumber)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}