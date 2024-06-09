using Dalamud.Plugin;

namespace WhereAmIAgain;

public sealed class WhereAmIAgainPlugin : IDalamudPlugin {
    public static Configuration Configuration { get; set; } = null!;
    public static ConfigurationWindow ConfigurationWindow { get; set; } = null!;
    public static DtrDisplay DtrDisplay { get; set; } = null!;
    
    public WhereAmIAgainPlugin(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();
            
        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        DtrDisplay = new DtrDisplay();
        ConfigurationWindow = new ConfigurationWindow();
    }

    public void Dispose() {
        ConfigurationWindow.Dispose();
        DtrDisplay.Dispose();
    }
}