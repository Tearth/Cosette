using System;

namespace Cosette.Arbiter.Logs
{
    public static class LogManager
    {
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now}] {message}");
        }

        public static void Log(string message, string from)
        {
            Console.WriteLine($"[{DateTime.Now}] ({from}) {message}");
        }
    }
}
