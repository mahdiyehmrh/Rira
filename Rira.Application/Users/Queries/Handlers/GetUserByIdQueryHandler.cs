using MediatR;
using Rira.Application.Common.Exceptions;
using Rira.Application.DTOs.Users;
using Rira.Application.Interfaces;

namespace Rira.Application.Users.Queries.Handlers;

public sealed class GetUserByIdQueryHandler(IUserRepository repository) : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdReadOnlyAsync(request.Id, cancellationToken) ?? throw new NotFoundException($"User {request.Id} was not found.");

        return UserDto.From(user);
    }
}
