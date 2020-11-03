using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Engine
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

            Write("uci");
            WaitForMessage("uciok");

            ApplyOptions();

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

        public void ApplyOptions()
        {
            SettingsLoader.Data.Options.ForEach(Write);
        }

        public BestMoveData Go(List<string> moves, int whiteClock, int blackClock)
        {
            var bestMoveData = new BestMoveData();
            var movesJoined = string.Join(' ', moves);

            if (moves.Count > 0)
            {
                Write($"position startpos moves {movesJoined}");
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

        public void WaitForMessage(string message)
        {
            while (Read() != message) ;
        }
    }
}
