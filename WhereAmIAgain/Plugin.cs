using Dalamud.Interface.GameFonts;
using Dalamud.Plugin;

namespace WhereAmIAgain;

public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Where am I again?";

    public Plugin (DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
            
        Service.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize(pluginInterface);

        Service.AxisFont = Service.PluginInterface.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Axis12));
        Service.DtrDisplay = new DtrDisplay();
        Service.ConfigurationWindow = new ConfigurationWindow();
    }

    public void Dispose()
    {
        Service.AxisFont.Dispose();
        Service.ConfigurationWindow.Dispose();
        Service.DtrDisplay.Dispose();
    }
}