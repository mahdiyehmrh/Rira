using Grpc.Core;
using Grpc.Net.Client;
using Rira.TestClient.Menu;
using System.Text;

namespace Rira.TestClient;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        var address = args.Length > 0 ? args[0] : "https://localhost:7086";
        using var channel = GrpcChannel.ForAddress(address);
        var client = new UserService.UserServiceClient(channel);

        while (true)
        {
            PrintMenu();
            Console.Write("Choice: ");
            var choice = Console.ReadLine();
            if (choice is null)
            {
                return;
            }

            switch (choice)
            {
                case "0":
                    return;
                case "1":
                    await RunSafely(() => CreateUser(client));
                    break;
                case "2":
                    await RunSafely(() => UpdateUser(client));
                    break;
                case "3":
                    await RunSafely(() => GetUser(client));
                    break;
                case "4":
                    await RunSafely(() => ListUsers(client));
                    break;
                case "5":
                    await RunSafely(() => DeleteUser(client));
                    break;
                default:
                    ConsoleColors.Warning("Invalid menu choice. Select 0 through 5.");
                    break;
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        ConsoleColors.Title("===== Rira User gRPC Client =====");
        ConsoleColors.Warning("Note: please type names in English letters only (the console may not display Persian text).");
        Console.WriteLine("1 = Create New User");
        Console.WriteLine("2 = Update User");
        Console.WriteLine("3 = Get User By Id");
        Console.WriteLine("4 = List Users");
        Console.WriteLine("5 = Delete User");
        Console.WriteLine("0 = Exit");
    }

    private static async Task CreateUser(UserService.UserServiceClient client)
    {
        var request = new CreateUserRequest
        {
            FirstName = ConsoleInput.ReadString("First name: "),
            LastName = ConsoleInput.ReadString("Last name: "),
            NationalCode = ConsoleInput.ReadString("National code: "),
            BirthDate = ConsoleInput.ReadString("Birth date (yyyy-MM-dd): ")
        };
        ResponsePrinter.Print(await client.CreateUserAsync(request));
    }

    private static async Task UpdateUser(UserService.UserServiceClient client)
    {
        var request = new UpdateUserRequest
        {
            Id = ConsoleInput.ReadInt("Id: "),
            FirstName = ConsoleInput.ReadString("First name: "),
            LastName = ConsoleInput.ReadString("Last name: "),
            NationalCode = ConsoleInput.ReadString("National code: "),
            BirthDate = ConsoleInput.ReadString("Birth date (yyyy-MM-dd): ")
        };
        ResponsePrinter.Print(await client.UpdateUserAsync(request));
    }

    private static async Task GetUser(UserService.UserServiceClient client)
    {
        var id = ConsoleInput.ReadInt("Id: ");
        ResponsePrinter.Print(await client.GetUserAsync(new GetUserRequest { Id = id }));
    }

    private static async Task ListUsers(UserService.UserServiceClient client)
    {
        var page = ConsoleInput.ReadOptionalInt("Page [1]: ", 1);
        var pageSize = ConsoleInput.ReadOptionalInt("Page size [20]: ", 20);
        ResponsePrinter.Print(await client.ListUsersAsync(new ListUsersRequest { Page = page, PageSize = pageSize }));
    }

    private static async Task DeleteUser(UserService.UserServiceClient client)
    {
        var id = ConsoleInput.ReadInt("Id: ");
        ResponsePrinter.Print(await client.DeleteUserAsync(new DeleteUserRequest { Id = id }));
    }

    private static async Task RunSafely(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (RpcException exception)
        {
            ResponsePrinter.Print(exception);
        }
        catch (Exception exception)
        {
            ConsoleColors.Error($"Operation failed: {exception.Message}");
        }
    }
}
