using MediatR;
using Rira.Application.Common.Exceptions;
using Rira.Application.Interfaces;

namespace Rira.Application.Users.Commands.Handlers;

public sealed class DeleteUserCommandHandler(IUserRepository repository) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException($"User {request.Id} was not found.");

        repository.Remove(user);

        await repository.SaveChangesAsync(cancellationToken);
    }
}
