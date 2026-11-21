using Grpc.Core;

namespace Rira.TestClient.Menu;

public static class ResponsePrinter
{
    public static void Print(UserReply user)
    {
        ConsoleColors.Success($"Id: {user.Id}");
        ConsoleColors.Success($"Name: {user.FirstName} {user.LastName}");
        ConsoleColors.Success($"National code: {user.NationalCode}");
        ConsoleColors.Success($"Birth date: {user.BirthDate}");
    }

    public static void Print(ListUsersReply users)
    {
        ConsoleColors.Title($"{"Id",-6} {"First name",-20} {"Last name",-20} {"National code",-14} Birth date");
        ConsoleColors.Title(new string('-', 82));
        foreach (var user in users.Items)
        {
            ConsoleColors.Success($"{user.Id,-6} {user.FirstName,-20} {user.LastName,-20} {user.NationalCode,-14} {user.BirthDate}");
        }

        ConsoleColors.Success($"Page: {users.Page}, PageSize: {users.PageSize}, TotalCount: {users.TotalCount}");
    }

    public static void Print(DeleteUserReply result)
    {
        if (result.Deleted)
        {
            ConsoleColors.Success("User deleted.");
        }
        else
        {
            ConsoleColors.Warning("User was not deleted.");
        }
    }

    public static void Print(RpcException exception)
    {
        ConsoleColors.Error($"{exception.StatusCode}: {exception.Status.Detail}");
        if (exception.StatusCode == StatusCode.Unavailable)
        {
            ConsoleColors.Warning("The server is not reachable. Start Rira.Api first.");
        }
    }
}
