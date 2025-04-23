namespace SetupWebhook;

/// <summary>
/// Utility class for printing styled messages to the console with emojis for better clarity.
/// </summary>
public static class ConsolePrinter
{
    /// <summary>
    /// Prints a success message in green with a ✅ emoji.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public static void Success(string message)
    {
        Print($"✅ {message}", ConsoleColor.Green);
    }

    /// <summary>
    /// Prints an error message in red with a ❌ emoji.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    public static void Error(string message)
    {
        Print($"❌ {message}", ConsoleColor.Red);
    }

    /// <summary>
    /// Prints a warning message in yellow with a ⚠️ emoji.
    /// </summary>
    public static void Warning(string message)
    {
        Print($"⚠️ {message}", ConsoleColor.Yellow);
    }

    /// <summary>
    /// Prints an informational message in yellow with a ℹ️ emoji.
    /// </summary>
    /// <param name="message">The info message to display.</param>
    public static void Info(string message)
    {
        Print($"ℹ️ {message}", ConsoleColor.Cyan);
    }

    /// <summary>
    /// Prints a debug message with a 🐛 emoji and no color.
    /// </summary>
    /// <param name="message">The debug message to display.</param>
    public static void Debug(string message)
    {
        Console.WriteLine($"🐛 {message}");
    }

    /// <summary>
    /// Prints a message in the specified console color.
    /// </summary>
    /// <param name="message">The message to print.</param>
    /// <param name="color">The console color to use.</param>
    private static void Print(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}