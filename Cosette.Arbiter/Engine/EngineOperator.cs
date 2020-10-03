using System.Diagnostics;

namespace Cosette.Arbiter.Engine
{
    public class EngineOperator
    {
        private string _name;
        private string _enginePath;
        private Process _engineProcess;

        public EngineOperator(string name, string path)
        {
            _name = name;
            _enginePath = path;
        }

        public void Init()
        {
            _engineProcess = Process.Start(new ProcessStartInfo
            {
                FileName = _enginePath,
                CreateNoWindow = true
            });
        }
    }
}
