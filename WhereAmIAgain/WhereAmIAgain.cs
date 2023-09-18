using Dalamud.Interface.GameFonts;
using Dalamud.Plugin;

namespace WhereAmIAgain;

public sealed class WhereAmIAgainPlugin : IDalamudPlugin
{
    public string Name => "Where am I again?";

    public static Configuration Configuration { get; set; } = null!;
    public static ConfigurationWindow ConfigurationWindow { get; set; } = null!;
    public static DtrDisplay DtrDisplay { get; set; } = null!;
    public static GameFontHandle AxisFont { get; set; } = null!;
    
    public WhereAmIAgainPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
            
        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        AxisFont = Service.PluginInterface.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Axis12));
        DtrDisplay = new DtrDisplay();
        ConfigurationWindow = new ConfigurationWindow();
    }

    public void Dispose()
    {
        AxisFont.Dispose();
        ConfigurationWindow.Dispose();
        DtrDisplay.Dispose();
    }
}