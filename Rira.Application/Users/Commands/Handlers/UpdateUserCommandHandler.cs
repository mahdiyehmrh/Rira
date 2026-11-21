using MediatR;
using Rira.Application.Common.Exceptions;
using Rira.Application.DTOs.Users;
using Rira.Application.Interfaces;

namespace Rira.Application.Users.Commands.Handlers;

public sealed class UpdateUserCommandHandler(IUserRepository repository, TimeProvider timeProvider) : IRequestHandler<UpdateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException($"User {request.Id} was not found.");

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        user.Update(request.FirstName, request.LastName, request.NationalCode, request.BirthDate, today);

        if (await repository.ExistsByNationalCodeAsync(user.NationalCode, user.Id, cancellationToken))
        {
            throw new ConflictException("A user with this NationalCode already exists.");
        }

        await repository.SaveChangesAsync(cancellationToken);

        return UserDto.From(user);
    }
}
