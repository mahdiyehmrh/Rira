using MediatR;
using Rira.Application.DTOs.Users;

namespace Rira.Application.Users.Queries;

public sealed record GetUserByIdQuery(int Id) : IRequest<UserDto>;
