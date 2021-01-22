using System;

namespace Cosette.Tuner.SelfPlay.Engine
{
    public class InfoData
    {
        public int Depth { get; set; }
        public int SelDepth { get; set; }
        public int Time { get; set; }
        public int ScoreCp { get; set; }
        public int ScoreMate { get; set; }
        public ulong Nodes { get; set; }
        public ulong Nps { get; set; }

        public static InfoData FromString(string data)
        {
            var splitData = data.Split(' ');
            return new InfoData
            {
                Depth = GetValue<int>("depth", splitData),
                SelDepth = GetValue<int>("seldepth", splitData),
                Time = GetValue<int>("time", splitData),
                ScoreCp = GetValue<int>("cp", splitData),
                ScoreMate = GetValue<int>("mate", splitData),
                Nodes = GetValue<ulong>("nodes", splitData),
                Nps = GetValue<ulong>("nps", splitData),
            };
        }

        private static T GetValue<T>(string name, string[] splitData)
        {
            for (var i = 0; i < splitData.Length; i++)
            {
                if (splitData[i] == name)
                {
                    return (T)Convert.ChangeType(splitData[i + 1], typeof(T));
                }
            }

            return default;
        }
    }
}
