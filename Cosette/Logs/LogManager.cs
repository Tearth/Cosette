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
#if LOGGER
            Log(Path.Combine(_basePath, _logFile), message);
#endif
        }

        public static void LogError(string message)
        {
#if LOGGER
            Log(Path.Combine(_basePath, $"error-{DateTime.Now.Ticks}.log"), message);
#endif
        }

        private static void Log(string filePath, string message)
        {
            using (var streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine(message);
            }
        }
    }
}
