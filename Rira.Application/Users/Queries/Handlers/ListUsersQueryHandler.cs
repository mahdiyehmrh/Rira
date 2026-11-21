using MediatR;
using Rira.Application.DTOs.Users;
using Rira.Application.Interfaces;

namespace Rira.Application.Users.Queries.Handlers;

public sealed class ListUsersQueryHandler(IUserRepository repository) : IRequestHandler<ListUsersQuery, PagedUsersDto>
{
    public async Task<PagedUsersDto> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await repository.ListAsync(page, pageSize, cancellationToken);

        return new PagedUsersDto(items.Select(UserDto.From).ToArray(), page, pageSize, totalCount);
    }
}
