using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cosette.Arbiter.Settings;

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
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            });

            Write("uci");
            WaitForMessage("uciok");

            SettingsLoader.Data.Options.ForEach(Write);

            Write("isready");
            WaitForMessage("readyok");
        }

        public void InitNewGame()
        {
            Write("ucinewgame");
            Write("isready");
            WaitForMessage("readyok");
        }

        public BestMoveData Go(List<string> moves)
        {
            var bestMoveData = new BestMoveData();
            var movesJoined = string.Join(' ', moves);

            if (moves.Count > 0)
            {
                Write($"position startpos moves {movesJoined}");
            }

            Write($"go movetime {SettingsLoader.Data.MillisecondsPerMove}");

            while (true)
            {
                try
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
                }
                catch
                {
                    Init();
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
