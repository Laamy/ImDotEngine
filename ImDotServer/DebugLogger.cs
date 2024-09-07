using System;

class DebugLogger
{
    public static void Log(string name, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logEntry = $"[{timestamp}, {name}] {message}";

        Console.WriteLine(logEntry);
    }

    public static void Warn(string message)
    {
        Log("DEBUG", message);
    }
}