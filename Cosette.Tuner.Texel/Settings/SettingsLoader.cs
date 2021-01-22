using System.IO;
using Newtonsoft.Json;

namespace Cosette.Tuner.Texel.Settings
{
    public static class SettingsLoader
    {
        public static SettingsModel Data { get; set; }

        public static void Init(string settingsPath)
        {
            using (var streamReader = new StreamReader(settingsPath))
            {
                var content = streamReader.ReadToEnd();
                Data = JsonConvert.DeserializeObject<SettingsModel>(content);
            }
        }
    }
}
