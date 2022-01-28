using System;
using System.Text.RegularExpressions;
using Dalamud.Plugin;
using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.IoC;
using Dalamud.Logging;
using System.IO;
using Lumina.Excel.GeneratedSheets;

using Dalamud.Data;
using System.Collections.Generic;
using Dalamud.Game.Text.SeStringHandling;
using System.Linq;


namespace WhereAmIAgain
{
    public sealed class Plugin : IDalamudPlugin, IDisposable
    {
        [PluginService]
        internal static ClientState ClientState { get; private set; }
        [PluginService]
        internal static Framework Framework { get; private set; }
        private readonly string[] fadingFootsteps = {
                "And lo, vile beasts did rise,",
                "Leaving naught in their wake but blood and ash.",
                "Thus did the first doom befall us.",
                "It would not, however, prove the last.",
                "For soon did the sun bend low, scorching earth and boiling seas.",
                "Thus did the second doom break us.",
        };
        private readonly string[] fadingHearth = {
                "Yet it was neither claw nor flame, but our very sins",
                "Stacked to the heavens",
                "Thus did the third doom undo us."
        };
        private List<TerritoryType> Territories;
        public string playerZone = "";
        private void UpdateLocation(Framework framework)
        {
            nui.Update();
            try
            {

                var localPlayer = ClientState.LocalPlayer;



                // Return early if data is not ready
                if (localPlayer is null)
                {
                    return;
                }

                var territoryId = ClientState.TerritoryType;
                var territoryName = "testing: name";
                var territoryRegion = "testing: region";

                if (territoryId != 0)
                {
                    // Read territory data from generated sheet
                    var territory = Territories.First(Row => Row.RowId == territoryId);
                    territoryName = territory.PlaceName.Value?.Name ?? "???";
                    territoryRegion = territory.PlaceNameRegion.Value?.Name ?? "The Void";

                }
                if (this.territoryName != territoryName || this.territoryRegion != territoryRegion)
                {
                    this.playerZone = "";
                }
                this.territoryName = territoryName;
                this.territoryRegion = territoryRegion;
                var locationString = $"{territoryName}";
                
                if (this.Configuration.DisplayTerritoryRegion)
                {
                    locationString = $"{locationString}, {territoryRegion}";
                }
                if (this.playerZone != "")
                {
                    locationString = $"{this.playerZone}, {locationString}";
                }
                nui.Update(locationString);
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Could not run OnUpdate.");
            }
        }
        public string Name => "Where am I again?";

        private const string commandName = "/waia";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        [PluginService]
        internal static DataManager DataManager { get; private set; }
        private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }
        public string territoryName;
        public string territoryRegion;
        private readonly NativeUIUtil nui;
        [PluginService]
        public static ToastGui ToastGui { get; private set; } = null!;
        public string LastUpdatedText = "";
        private Regex rx = new Regex(@"[.!\]:]", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] GameGui gameGui,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            this.PluginUi = new PluginUI(this.Configuration);

            this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Displays the local in-game zone next to your server info."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;


            Framework.Update += UpdateLocation;
            Territories = DataManager.GetExcelSheet<TerritoryType>().ToList();
            nui = new NativeUIUtil(Configuration, gameGui);
            nui.Init();
            ToastGui.Toast += this.OnToast;
        }

        public void Dispose()
        {
            this.PluginUi.Dispose();
            nui.Dispose();
            this.CommandManager.RemoveHandler(commandName);
            ToastGui.Toast -= this.OnToast;
        }

        private void OnCommand(string command, string args)
        {
            this.PluginUi.SettingsVisible = true;
        }

        private void DrawUI()
        {
            this.PluginUi.Draw(this.territoryName, this.territoryRegion);
        }

        private void DrawConfigUI()
        {
            this.PluginUi.SettingsVisible = true;
        }

        private void OnToast(ref SeString message, ref ToastOptions options, ref bool ishandled)
        {
            var text = $"{message}";
            // special handling for amaurot weirdness
            if (this.territoryName == "Amaurot")
            {
                
                 if (Array.FindAll(this.fadingFootsteps, label => text.Contains(label)).Length > 0)
                {
                    this.playerZone = $"\"{text}\" Fading Footsteps";
                } else
                {
                    if (Array.FindAll(this.fadingHearth, label => text.Contains(label)).Length > 0)
                    {
                        this.playerZone = $"\"{text}\" Fading Hearth";
                    }
                }
                return;
            }
            MatchCollection matches = this.rx.Matches(text);
            if (matches.Count == 0)
            {
                this.playerZone = text;
            }
        }
    }
}
