namespace Rira.TestClient.Menu;

public static class ConsoleInput
{
    public static string ReadString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    public static int ReadInt(string prompt)
    {
        while (true)
        {
            var input = ReadString(prompt);
            if (int.TryParse(input, out var value))
            {
                return value;
            }

            ConsoleColors.Warning("Please enter a number.");
        }
    }

    public static int ReadOptionalInt(string prompt, int defaultValue)
    {
        while (true)
        {
            var input = ReadString(prompt);
            if (string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }

            if (int.TryParse(input, out var value))
            {
                return value;
            }

            ConsoleColors.Warning("Please enter a number or leave blank for the default.");
        }
    }
}
