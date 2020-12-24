using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Cosette.Tuner.Texel.Genetics.Epd
{
    public class EpdLoader
    {
        public List<PositionData> Positions { get; set; }

        public void Load(string epdPath)
        {
            Positions = new List<PositionData>();

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

                    Positions.Add(new PositionData
                    {
                        Fen = trimmedLine,
                        Result = GetGameResult(result)
                    });
                }
            }
        }

        private GameResult GetGameResult(string result)
        {
            switch (result)
            {
                case "\"1-0\"": return GameResult.WhiteWon;
                case "\"0-1\"": return GameResult.BlackWon;
                case "\"1/2-1/2\"": return GameResult.Draw;
            }

            throw new InvalidOperationException();
        }
    }
}
