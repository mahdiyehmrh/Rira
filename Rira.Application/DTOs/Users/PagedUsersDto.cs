namespace Rira.Application.DTOs.Users;

public sealed record PagedUsersDto(IReadOnlyList<UserDto> Items, int Page, int PageSize, int TotalCount);
