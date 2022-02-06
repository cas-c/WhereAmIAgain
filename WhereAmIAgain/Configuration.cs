using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace WhereAmIAgain
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool DisplayTerritoryRegion { get; set; } = true;
        public bool DisplayTerritoryName { get; set; } = true;
        public bool DisplayPlaceName { get; set; } = true;
        public bool DisplayZoneSeparator { get; set; } = true;
        public string ZoneSeparator { get; set; } = ",";
        public bool DisplayZoneSeparator2 { get; set; } = true;
        public string ZoneSeparator2 { get; set; } = ",";
        public bool DisplayZoneLead { get; set; } = false;
        public string DisplayZoneLeadingText { get; set; } = "";
        public bool DisplayZoneTailing { get; set; } = false;
        public string DisplayZoneTailingText { get; set; } = "";
        public bool RemoveDuplicates { get; set; } = true;

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}
