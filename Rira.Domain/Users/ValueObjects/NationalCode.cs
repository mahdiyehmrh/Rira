using System.Text;
using Rira.Domain.Common;

namespace Rira.Domain.Users.ValueObjects;

public sealed record NationalCode
{
    private NationalCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<NationalCode> TryCreate(string? value, string fieldName)
    {
        var normalized = Normalize(value);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<NationalCode>.Failure([new DomainError(fieldName, "User.NationalCode.Required", "NationalCode is required.")]);
        }

        if (normalized.Length != 10 || normalized.Any(character => character is < '0' or > '9'))
        {
            return Result<NationalCode>.Failure([new DomainError(fieldName, "User.NationalCode.InvalidFormat", "NationalCode must contain exactly 10 digits.")]);
        }

        if (normalized.All(character => character == normalized[0]))
        {
            return Result<NationalCode>.Failure([new DomainError(fieldName, "User.NationalCode.InvalidFormat", "NationalCode cannot contain the same digit repeated.")]);
        }

        var sum = 0;
        for (var index = 0; index < 9; index++)
        {
            sum += (normalized[index] - '0') * (10 - index);
        }

        var remainder = sum % 11;
        var expected = remainder < 2 ? remainder : 11 - remainder;
        if (normalized[9] - '0' != expected)
        {
            return Result<NationalCode>.Failure([new DomainError(fieldName, "User.NationalCode.InvalidChecksum", "NationalCode checksum is invalid.")]);
        }

        return Result<NationalCode>.Success(new NationalCode(normalized));
    }

    public static NationalCode Create(string? value)
    {
        var result = TryCreate(value, "NationalCode");
        if (!result.IsSuccess())
        {
            throw new DomainException(result.Errors);
        }

        return result.Value!;
    }

    public static NationalCode FromDatabase(string value)
    {
        return new(value);
    }

    private static string Normalize(string? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        var trimmed = value.Trim();
        var builder = new StringBuilder(trimmed.Length);
        foreach (var character in trimmed)
        {
            builder.Append(character switch
            {
                >= '\u06F0' and <= '\u06F9' => (char)('0' + character - '\u06F0'),
                >= '\u0660' and <= '\u0669' => (char)('0' + character - '\u0660'),
                _ => character
            });
        }

        return builder.ToString();
    }
}
