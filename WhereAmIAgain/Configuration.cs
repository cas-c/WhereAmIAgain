using Dalamud.Configuration;

namespace WhereAmIAgain;

public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public string FormatString = "{0}, {1}, {2}, {3}";
    public string TooltipFormatString = "{0}, {1}, {2}, {3}";
    public bool ShowInstanceNumber = true;
    public bool UsePreciseHousingLocation = false;

    public void Save() => Service.PluginInterface.SavePluginConfig(this);
}