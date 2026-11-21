using MediatR;

namespace Rira.Application.Users.Commands;

public sealed record DeleteUserCommand(int Id) : IRequest;
