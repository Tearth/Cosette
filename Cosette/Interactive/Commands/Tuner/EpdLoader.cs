using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cosette.Engine.Fen;

namespace Cosette.Interactive.Commands.Tuner
{
    public class EpdLoader
    {
        public List<EpdPositionData> Load(string epdPath)
        {
            var positions = new List<EpdPositionData>();
            using (var streamReader = new StreamReader(epdPath))
            {
                while(!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();

                    var trimmedLine = line.Trim(';');
                    var chunks = trimmedLine.Split(' ');
                    var result = chunks.Last();

                    if (string.IsNullOrWhiteSpace(result))
                    {
                        continue;
                    }

                    positions.Add(new EpdPositionData
                    {
                        Fen = trimmedLine,
                        Result = GetGameResult(result)
                    });
                }
            }

            return positions;
        }

        private EpdGameResult GetGameResult(string result)
        {
            switch (result)
            {
                case "\"1-0\"": return EpdGameResult.WhiteWon;
                case "\"0-1\"": return EpdGameResult.BlackWon;
                case "\"1/2-1/2\"": return EpdGameResult.Draw;
            }

            throw new InvalidOperationException();
        }
    }
}
