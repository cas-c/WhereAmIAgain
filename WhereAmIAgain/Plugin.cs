using Dalamud.Plugin;

namespace WhereAmIAgain
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Where am I again?";

        public Plugin (DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();
            
            Service.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(pluginInterface);

            Service.DtrDisplay = new DtrDisplay();
            Service.ConfigurationWindow = new ConfigurationWindow();
        }

        public void Dispose()
        {
            Service.ConfigurationWindow.Dispose();
            Service.DtrDisplay.Dispose();
        }
    }
}
