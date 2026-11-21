using MediatR;
using Rira.Application.Common.Exceptions;
using Rira.Application.DTOs.Users;
using Rira.Application.Interfaces;
using Rira.Domain.Users;

namespace Rira.Application.Users.Commands.Handlers;

public sealed class CreateUserCommandHandler(IUserRepository repository, TimeProvider timeProvider) : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var user = User.Create(request.FirstName, request.LastName, request.NationalCode, request.BirthDate, today);

        if (await repository.ExistsByNationalCodeAsync(user.NationalCode, null, cancellationToken))
        {
            throw new ConflictException("A user with this NationalCode already exists.");
        }

        await repository.AddAsync(user, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return UserDto.From(user);
    }
}
