namespace SetupWebhook;

/// <summary>
/// Utility class for printing styled messages to the console.
/// </summary>
public static class ConsolePrinter
{
    /// <summary>
    /// Prints a success message in green with [SUC] level.
    /// </summary>
    public static void Success(string message)
    {
        Print($"[SUC] {message}", ConsoleColor.Green);
    }

    /// <summary>
    /// Prints an error message in red with [ERR] level.
    /// </summary>
    public static void Error(string message)
    {
        Print($"[ERR] {message}", ConsoleColor.Red);
    }

    /// <summary>
    /// Prints an informational message in cyan with [INF] level.
    /// </summary>
    public static void Info(string message)
    {
        Print($"[INF] {message}", ConsoleColor.Cyan);
    }

    /// <summary>
    /// Prints a warning message in yellow with [WRN] level.
    /// </summary>
    public static void Warning(string message)
    {
        Print($"[WRN] {message}", ConsoleColor.Yellow);
    }

    /// <summary>
    /// Prints a debug message with [DBG] level and no color.
    /// </summary>
    public static void Debug(string message)
    {
        Console.WriteLine($"[DBG] {message}");
    }

    /// <summary>
    /// Prints a message in the specified console color.
    /// </summary>
    private static void Print(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}