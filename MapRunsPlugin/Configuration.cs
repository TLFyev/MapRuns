using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace MapRuns
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool TrackGil { get; set; } = true;
        public bool TrackPoetics { get; set; } = true;
        public bool TrackUncapped { get; set; } = true;
        public bool TrackCapped { get; set; } = true;
        public bool TrackItemsWithRolls { get; set; } = true;
        public bool TrackItemsWithoutRolls { get; set; } = false;

        public void Save()
        {
            Services.PluginInterface.SavePluginConfig(this);
        }
    }
}
