using MediatR;
using Rira.Application.DTOs.Users;

namespace Rira.Application.Users.Commands;

public sealed record CreateUserCommand(string FirstName, string LastName, string NationalCode, DateOnly BirthDate) : IRequest<UserDto>;
