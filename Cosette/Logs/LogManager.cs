using System;
using System.IO;

namespace Cosette.Logs
{
    public static class LogManager
    {
        private static string _logFile;
        private static string _basePath;

        static LogManager()
        {
#if LOGGER
            _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            _logFile = Path.Combine(_basePath, $"info-{DateTime.Now.Ticks}.log");

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
#endif
        }

        public static void LogInfo(string message)
        {
            Log(Path.Combine(_basePath, _logFile), message);
        }

        public static void LogError(string message)
        {
            Log(Path.Combine(_basePath, $"error-{DateTime.Now.Ticks}.log"), message);
        }

        private static void Log(string filePath, string message)
        {
#if LOGGER
            using (var streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine(message);
            }
#endif
        }
    }
}
