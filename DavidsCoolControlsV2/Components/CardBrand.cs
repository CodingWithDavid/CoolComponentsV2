using System.Text;
using System.Text.RegularExpressions;

namespace DavidsCoolControlsV2.Components;

public enum CardField { None, Number, Name, Expiry, Cvc }

public sealed record CardBrand(string Id, string DisplayName, Regex Prefix, int[] Lengths, int[] Groups, int CvcLength)
{
    public int MaxLength => Lengths[^1];

    public bool IsKnown => Id != "unknown";

    public static readonly CardBrand Unknown = new("unknown", "Card", new("^$"), [16], [4, 4, 4, 4], 3);
}

public sealed record CardSummary(string Brand, string Last4, string NameOnCard, string Expiry);

public static class CardTypes
{
    // Order matters: more specific prefixes come first.
    public static readonly IReadOnlyList<CardBrand> All =
    [
        new("amex", "American Express", new("^3[47]"), [15], [4, 6, 5], 4),
        new("diners", "Diners Club", new("^3(0[0-5]|[689])"), [14], [4, 6, 4], 3),
        new("jcb", "JCB", new("^35"), [16, 17, 18, 19], [4, 4, 4, 4, 3], 3),
        new("visa", "Visa", new("^4"), [13, 16, 19], [4, 4, 4, 4, 3], 3),
        new("mastercard", "Mastercard", new("^(5[1-5]|2[2-7])"), [16], [4, 4, 4, 4], 3),
        new("discover", "Discover", new("^(6011|65|64[4-9])"), [16, 19], [4, 4, 4, 4, 3], 3),
        new("unionpay", "UnionPay", new("^62"), [16, 17, 18, 19], [4, 4, 4, 4, 3], 3),
    ];

    public static CardBrand Detect(string digits) =>
        All.FirstOrDefault(b => b.Prefix.IsMatch(digits)) ?? CardBrand.Unknown;

    public static string Digits(string? value, int max = int.MaxValue)
    {
        var builder = new StringBuilder();
        foreach (var c in value ?? string.Empty)
        {
            if (char.IsAsciiDigit(c) && builder.Length < max)
            {
                builder.Append(c);
            }
        }
        return builder.ToString();
    }

    public static string FormatNumber(string digits, CardBrand brand) =>
        string.Join(' ', Group(digits, brand.Groups));

    /// <summary>Splits text into the brand's digit groups, ignoring empty trailing groups.</summary>
    public static IEnumerable<string> Group(string text, int[] groups)
    {
        var index = 0;
        foreach (var size in groups)
        {
            if (index >= text.Length) yield break;
            var take = Math.Min(size, text.Length - index);
            yield return text.Substring(index, take);
            index += take;
        }
        if (index < text.Length) yield return text[index..];
    }

    /// <summary>The length the card preview should show: the brand's usual length, or longer if more digits were typed.</summary>
    public static int DisplayLength(string digits, CardBrand brand)
    {
        var typical = brand.Lengths.Contains(16) ? 16 : brand.Lengths[0];
        return brand.Lengths.FirstOrDefault(l => l >= Math.Max(digits.Length, typical), brand.MaxLength);
    }

    public static string FormatExpiry(string? value)
    {
        var digits = Digits(value);
        if (digits.Length > 0 && digits[0] > '1')
        {
            digits = "0" + digits;
        }
        digits = digits[..Math.Min(digits.Length, 4)];
        return digits.Length > 2 ? $"{digits[..2]}/{digits[2..]}" : digits;
    }

    public static bool PassesLuhn(string digits)
    {
        var sum = 0;
        var doubleIt = false;
        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var d = digits[i] - '0';
            if (doubleIt && (d *= 2) > 9) d -= 9;
            sum += d;
            doubleIt = !doubleIt;
        }
        return digits.Length > 0 && sum % 10 == 0;
    }

    public static string? ValidateNumber(string digits, CardBrand brand)
    {
        if (digits.Length == 0) return "Enter a card number.";
        if (!brand.IsKnown) return "This card type isn't recognized. Check the first digits.";
        if (!brand.Lengths.Contains(digits.Length))
        {
            var lengths = string.Join(" or ", brand.Lengths);
            return $"{brand.DisplayName} numbers are {lengths} digits.";
        }
        return PassesLuhn(digits) ? null : "This card number isn't valid. Check for typos.";
    }

    public static string? ValidateExpiry(string expiry, DateOnly today)
    {
        var digits = Digits(expiry);
        if (digits.Length != 4) return "Enter the expiry date as MM/YY.";
        var month = int.Parse(digits[..2]);
        var year = 2000 + int.Parse(digits[2..]);
        if (month is < 1 or > 12) return "Enter a month from 01 to 12.";
        if (year < today.Year || (year == today.Year && month < today.Month)) return "This card has expired.";
        if (year > today.Year + 20) return "Check the expiry year.";
        return null;
    }

    public static string? ValidateCvc(string cvc, CardBrand brand) =>
        cvc.Length == brand.CvcLength ? null : $"Enter the {brand.CvcLength}-digit security code.";
}
