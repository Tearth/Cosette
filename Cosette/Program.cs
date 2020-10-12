using System;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Moves.Magic;
using Cosette.Interactive;
using Cosette.Logs;

namespace Cosette
{
    public class Program
    {
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            HashTableAllocator.Allocate();
            MagicBitboards.InitWithInternalKeys();

            new InteractiveConsole().Run();
        }

        public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.LogError(e.ExceptionObject.ToString());
        }
    }
}
