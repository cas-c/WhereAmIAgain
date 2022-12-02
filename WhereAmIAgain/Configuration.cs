using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace WhereAmIAgain
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public string FormatString = "{0}, {1}, {2}, {3}";
        public bool ShowInstanceNumber = true;

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;
        public void Initialize(DalamudPluginInterface inputPluginInterface) => pluginInterface = inputPluginInterface;
        public void Save() => pluginInterface!.SavePluginConfig(this);
    }
}
