using Dalamud.Configuration;

namespace WhereAmIAgain;

public class Configuration : IPluginConfiguration {

	public string FormatString = "{0}, {1}, {2}, {3}";
	public bool ShowInstanceNumber = true;
	public string TooltipFormatString = "{0}, {1}, {2}, {3}";
	public bool UsePreciseHousingLocation = false;
	public int Version { get; set; } = 0;

	public void Save()
		=> Service.PluginInterface.SavePluginConfig(this);
}