using System.Diagnostics;

namespace Cosette.Tuner.Texel.Engine
{
    public class EngineOperator
    {
        private string _enginePath;
        private string _engineArguments;
        private Process _engineProcess;

        public EngineOperator(string path, string arguments)
        {
            _enginePath = path;
            _engineArguments = arguments;
        }

        public void Init()
        {
            _engineProcess = Process.Start(new ProcessStartInfo
            {
                FileName = _enginePath,
                Arguments = _engineArguments,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            });
        }

        public int Evaluate(string fen)
        {
            Write($"evaluateraw {fen}");

            while (true)
            {
                var response = Read();
                return int.Parse(response);
            }
        }

        public void Write(string message)
        {
            _engineProcess.StandardInput.WriteLine(message);
        }

        public string Read()
        {
            return _engineProcess.StandardOutput.ReadLine();
        }
    }
}
