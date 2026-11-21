using MediatR;
using Rira.Application.DTOs.Users;

namespace Rira.Application.Users.Queries;

public sealed record ListUsersQuery(int Page = 1, int PageSize = 20) : IRequest<PagedUsersDto>;
