using System.Globalization;
using Grpc.Core;
using MediatR;
using Rira.Application.DTOs.Users;
using Rira.Application.Users.Commands;
using Rira.Application.Users.Queries;
using Rira.Domain.Common;

namespace Rira.Api.Services;

public sealed class UserGrpcService(ISender sender) : UserService.UserServiceBase
{
    public override async Task<UserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var command = new CreateUserCommand(request.FirstName, request.LastName, request.NationalCode, ParseDate(request.BirthDate));
        var dto = await sender.Send(command, context.CancellationToken);
        return ToReply(dto);
    }

    public override async Task<UserReply> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var dto = await sender.Send(new GetUserByIdQuery(request.Id), context.CancellationToken);
        return ToReply(dto);
    }

    public override async Task<ListUsersReply> ListUsers(ListUsersRequest request, ServerCallContext context)
    {
        var query = new ListUsersQuery(request.Page, request.PageSize);
        var page = await sender.Send(query, context.CancellationToken);
        var reply = new ListUsersReply { Page = page.Page, PageSize = page.PageSize, TotalCount = page.TotalCount };
        reply.Items.AddRange(page.Items.Select(ToReply));
        return reply;
    }

    public override async Task<UserReply> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var command = new UpdateUserCommand(request.Id, request.FirstName, request.LastName, request.NationalCode, ParseDate(request.BirthDate));
        var dto = await sender.Send(command, context.CancellationToken);
        return ToReply(dto);
    }

    public override async Task<DeleteUserReply> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        await sender.Send(new DeleteUserCommand(request.Id), context.CancellationToken);
        return new DeleteUserReply { Deleted = true };
    }

    private static DateOnly ParseDate(string value)
    {
        return DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : throw new DomainException(new DomainError("BirthDate", "User.BirthDate.InvalidFormat", "BirthDate must use the yyyy-MM-dd format."));

    }

    private static UserReply ToReply(UserDto user)
    {
        return new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            NationalCode = user.NationalCode,
            BirthDate = user.BirthDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        };
    }
}
