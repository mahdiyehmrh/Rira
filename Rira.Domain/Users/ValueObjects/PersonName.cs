using System.Text.RegularExpressions;
using Rira.Domain.Common;

namespace Rira.Domain.Users.ValueObjects;

public sealed partial record PersonName
{
    private PersonName(string value)
    {
        Value = value;
    }

    public const int MinLength = 2;
    public const int MaxLength = 100;
    public string Value { get; }

    public static Result<PersonName> TryCreate(string? value, string fieldName)
    {
        var normalized = Normalize(value);
        var errors = new List<DomainError>();
        var prefix = $"User.{fieldName}";

        if (string.IsNullOrWhiteSpace(normalized))
        {
            errors.Add(new DomainError(fieldName, $"{prefix}.Required", $"{fieldName} is required."));
        }
        else
        {
            if (normalized.Length < MinLength)
            {
                errors.Add(new DomainError(fieldName, $"{prefix}.TooShort", $"{fieldName} must be at least {MinLength} characters."));
            }

            if (normalized.Length > MaxLength)
            {
                errors.Add(new DomainError(fieldName, $"{prefix}.TooLong", $"{fieldName} must be at most {MaxLength} characters."));
            }

            if (!IsAllowedName().IsMatch(normalized))
            {
                errors.Add(new DomainError(fieldName, $"{prefix}.InvalidFormat", $"{fieldName} may contain only Persian or English letters, spaces, hyphens, apostrophes, and ZWNJ, and must start and end with a letter."));
            }
        }

        return errors.Count == 0
            ? Result<PersonName>.Success(new PersonName(normalized))
            : Result<PersonName>.Failure(errors);
    }

    public static PersonName Create(string? value, string fieldName)
    {
        var result = TryCreate(value, fieldName);
        if (!result.IsSuccess())
        {
            throw new DomainException(result.Errors);
        }

        return result.Value!;
    }

    public static PersonName FromDatabase(string value)
    {
        return new(value); 
    }

    private static string Normalize(string? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return CollapseSpaces().Replace(value.Trim().Replace('\u064A', '\u06CC').Replace('\u0643', '\u06A9'), " ");
    }

    [GeneratedRegex("^[A-Za-z\\u0621-\\u063A\\u0641-\\u064A\\u066E-\\u066F\\u0671-\\u06D3\\u06D5\\u06EE-\\u06EF\\u06FA-\\u06FC\\u06FF](?:[A-Za-z\\u0621-\\u063A\\u0641-\\u064A\\u066E-\\u066F\\u0671-\\u06D3\\u06D5\\u06EE-\\u06EF\\u06FA-\\u06FC\\u06FF \\u200C\\-']*[A-Za-z\\u0621-\\u063A\\u0641-\\u064A\\u066E-\\u066F\\u0671-\\u06D3\\u06D5\\u06EE-\\u06EF\\u06FA-\\u06FC\\u06FF])?$", RegexOptions.CultureInvariant)]
    private static partial Regex IsAllowedName();

    [GeneratedRegex(" +", RegexOptions.CultureInvariant)]
    private static partial Regex CollapseSpaces();
}
