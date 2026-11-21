using Rira.Domain.Common;
using Rira.Domain.Users.ValueObjects;

namespace Rira.Domain.Users;

public sealed class User
{
    private User()
    {
    }

    public int Id { get; private set; }
    public PersonName FirstName { get; private set; } = null!;
    public PersonName LastName { get; private set; } = null!;
    public NationalCode NationalCode { get; private set; } = null!;
    public DateOnly BirthDate { get; private set; }

    public static User Create(string? firstName, string? lastName, string? nationalCode, DateOnly birthDate, DateOnly today)
    {
        var validated = Validate(firstName, lastName, nationalCode, birthDate, today);
        return new User
        {
            FirstName = validated.FirstName,
            LastName = validated.LastName,
            NationalCode = validated.NationalCode,
            BirthDate = birthDate
        };
    }

    public void Update(string? firstName, string? lastName, string? nationalCode, DateOnly birthDate, DateOnly today)
    {
        var validated = Validate(firstName, lastName, nationalCode, birthDate, today);
        FirstName = validated.FirstName;
        LastName = validated.LastName;
        NationalCode = validated.NationalCode;
        BirthDate = birthDate;
    }

    private static (PersonName FirstName, PersonName LastName, NationalCode NationalCode) Validate(string? firstName, string? lastName, string? nationalCode, DateOnly birthDate, DateOnly today)
    {
        var firstNameResult = PersonName.TryCreate(firstName, "FirstName");
        var lastNameResult = PersonName.TryCreate(lastName, "LastName");
        var nationalCodeResult = NationalCode.TryCreate(nationalCode, "NationalCode");
        var errors = firstNameResult.Errors
            .Concat(lastNameResult.Errors)
            .Concat(nationalCodeResult.Errors)
            .ToList();

        if (birthDate > today)
        {
            errors.Add(new DomainError("BirthDate", "User.BirthDate.InFuture", "BirthDate cannot be in the future."));
        }

        if (birthDate < new DateOnly(1900, 1, 1))
        {
            errors.Add(new DomainError("BirthDate", "User.BirthDate.TooOld", "BirthDate cannot be before 1900-01-01."));
        }

        if (errors.Count > 0)
        {
            throw new DomainException(errors);
        }

        return (firstNameResult.Value!, lastNameResult.Value!, nationalCodeResult.Value!);
    }
}
