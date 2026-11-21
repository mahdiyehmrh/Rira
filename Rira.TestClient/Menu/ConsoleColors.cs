namespace Rira.TestClient.Menu;

public static class ConsoleColors
{
    public static void WriteLine(string text, ConsoleColor color)
    {
        if (Console.IsOutputRedirected)
        {
            Console.WriteLine(text);
            return;
        }

        var previousColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }

    public static void Success(string text)
    {
        WriteLine(text, ConsoleColor.Green);
    }

    public static void Error(string text)
    {
        WriteLine(text, ConsoleColor.Red);
    }

    public static void Warning(string text)
    {
        WriteLine(text, ConsoleColor.Yellow);
    }

    public static void Title(string text)
    {
        WriteLine(text, ConsoleColor.Cyan);
    }
}
