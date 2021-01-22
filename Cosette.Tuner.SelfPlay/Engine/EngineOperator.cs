using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cosette.Tuner.SelfPlay.Settings;

namespace Cosette.Tuner.SelfPlay.Engine
{
    public class EngineOperator
    {
        private string _enginePath;
        private string _engineArguments;
        private Process _engineProcess;
        private Dictionary<string, string> _options;

        public EngineOperator(string path, string arguments)
        {
            _enginePath = path;
            _engineArguments = arguments;
            _options = new Dictionary<string, string>();
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

            Write("uci");
            WaitForMessage("uciok");

            Write("isready");
            WaitForMessage("readyok");
        }

        public void Restart()
        {
            if (!_engineProcess.HasExited)
            {
                _engineProcess.Close();
            }

            Init();
            ApplyOptions();
        }

        public void InitNewGame()
        {
            Write("ucinewgame");
            Write("isready");
            WaitForMessage("readyok");
        }

        public void SetOption(string name, string value)
        {
            _options[name] = value;
        }

        public void ApplyOptions()
        {
            SettingsLoader.Data.Options.ForEach(Write);
            foreach (var option in _options)
            {
                Write($"setoption name {option.Key} value {option.Value}");
            }

            Write("isready");

            if (!WaitForMessage("readyok"))
            {
                throw new Exception("Invalid option passed to the engine");
            }
        }

        public BestMoveData Go(List<string> moves, int whiteClock, int blackClock)
        {
            var bestMoveData = new BestMoveData();
            var movesJoined = string.Join(' ', moves);

            if (moves.Count > 0)
            {
                Write($"position startpos moves {movesJoined}");
                Write("isready");
                WaitForMessage("readyok");
            }

            Write($"go wtime {whiteClock} btime {blackClock} winc {SettingsLoader.Data.IncTime} binc {SettingsLoader.Data.IncTime}");

            while (true)
            {
                var response = Read();
                if (response.StartsWith("info depth"))
                {
                    bestMoveData.LastInfoData = InfoData.FromString(response);
                }
                else if (response.StartsWith("bestmove"))
                {
                    bestMoveData.BestMove = response.Split(' ')[1];
                    break;
                }
                else if (response.StartsWith("error"))
                {
                    return null;
                }
            }

            return bestMoveData;
        }

        public void Write(string message)
        {
            _engineProcess.StandardInput.WriteLine(message);
        }

        public string Read()
        {
            return _engineProcess.StandardOutput.ReadLine();
        }

        public bool WaitForMessage(string message)
        {
            while (true)
            {
                var response = Read();
                if (response.StartsWith("error"))
                {
                    return false;
                }
                
                if (response == message)
                {
                    return true;
                }
            }
        }
    }
}
