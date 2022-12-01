using System;
using Dalamud.Plugin;
using Dalamud.IoC;

namespace WhereAmIAgain
{
    public sealed class Plugin : IDalamudPlugin, IDisposable
    {
        public string Name => "Where am I again?";

        public Plugin ([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
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
