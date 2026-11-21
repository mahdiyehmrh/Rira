using Rira.Domain.Users;

namespace Rira.Application.DTOs.Users;

public sealed record UserDto(int Id, string FirstName, string LastName, string NationalCode, DateOnly BirthDate)
{
    public static UserDto From(User user)
    {
        return new(user.Id, user.FirstName.Value, user.LastName.Value, user.NationalCode.Value, user.BirthDate);
    }
}
