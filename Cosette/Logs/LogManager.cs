using System;
using System.IO;

namespace Cosette.Logs
{
    public static class LogManager
    {
        private static readonly string _logFile;
        private static readonly string _basePath;

        private static StreamWriter _infoLogStreamWriter;

        static LogManager()
        {
#if LOGGER
            _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            _logFile = Path.Combine(_basePath, $"info-{DateTime.Now.Ticks}.log");

            _infoLogStreamWriter = new StreamWriter(_logFile);

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
#endif
        }

        public static void LogInfo(string message)
        {
#if LOGGER
            _infoLogStreamWriter.WriteLine(message);
#endif
        }

        public static void LogError(string message)
        {
#if LOGGER
            using (var streamWriter = new StreamWriter(Path.Combine(_basePath, $"error-{DateTime.Now.Ticks}.log"), true))
            {
                streamWriter.WriteLine(message);
            }
#endif
        }
    }
}
