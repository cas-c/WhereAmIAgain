using System;
using System.Text.RegularExpressions;
using Dalamud.Plugin;
using Dalamud.Game;
using Dalamud.Game.Text;
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
using Dalamud.Game.Gui.Dtr;

namespace WhereAmIAgain
{
    public sealed class Plugin : IDalamudPlugin, IDisposable
    {
        //private string ConstName => "Where am I again?";
        public string Name => "Where am I again?";
        private const string commandName = "/waia";

        [PluginService] internal static ClientState ClientState { get; private set; }
        [PluginService] internal static DataManager DataManager { get; private set; }
        [PluginService] public static DtrBar DtrBar { get; private set; }
        [PluginService] internal static Framework Framework { get; private set; }
        [PluginService] public static ToastGui ToastGui { get; private set; } = null!;

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }

        private DtrBarEntry dtrEntry;
        private List<TerritoryType> Territories;
        private static char RightArrow = (char)SeIconChar.ArrowRight;
        private Regex rx = new Regex($@"[.!\]:{RightArrow}]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string playerZone = "";
        public string territoryName = "";
        public string territoryRegion = "";
        public string LastUpdatedText = "";     

        public Plugin (
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            this.PluginUi = new PluginUI(this);

            this.CommandManager.AddHandler(commandName, new CommandInfo(this.OnCommand)
            {
                HelpMessage = "Displays the local in-game zone next to your server info."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            this.Territories = DataManager.GetExcelSheet<TerritoryType>().ToList();
            this.dtrEntry = DtrBar.Get(this.Name);
            Framework.Update += UpdateLocation;
            ToastGui.Toast += this.OnToast;
        }

        public void Dispose()
        {
            this.PluginInterface.UiBuilder.Draw -= DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
            this.PluginUi.Dispose();
            this.CommandManager.RemoveHandler(commandName);
            ToastGui.Toast -= this.OnToast;
            Framework.Update -= this.UpdateLocation;
        }


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
        private void UpdateLocation(Framework framework)
        {
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
                var locationString = "";
                var separator = "";
                if (this.Configuration.DisplayZoneSeparator)
                {
                    separator = this.Configuration.ZoneSeparator;
                }
                var separator2 = "";
                if (this.Configuration.DisplayZoneSeparator2)
                {
                    separator2 = this.Configuration.ZoneSeparator2;
                }

                if (this.Configuration.DisplayPlaceName)
                {
                    locationString = this.playerZone;
                    
                };
                var leadingText = "";
                if (this.Configuration.DisplayZoneLead)
                {
                    leadingText = this.Configuration.DisplayZoneLeadingText;
                }
                var tailingText = "";
                if (this.Configuration.DisplayZoneTailing)
                {
                    tailingText = this.Configuration.DisplayZoneTailingText;
                }


                if (this.Configuration.DisplayTerritoryName)
                {
                    var hideDueToDuplicates = false;
                    if (this.Configuration.RemoveDuplicates)
                    {

                        var rx = new Regex(this.territoryName, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        var duplicateTerritoryName = rx.Matches(this.playerZone);
                        hideDueToDuplicates = duplicateTerritoryName.Count != 0;
                    }
                    if (!hideDueToDuplicates)
                    {

                        if (this.Configuration.DisplayPlaceName && locationString != "")
                        {
                            locationString = $"{locationString}{separator} {this.territoryName}";
                        }
                        else
                        {
                            locationString = this.territoryName;
                        }
                    }
                }

                if (this.Configuration.DisplayTerritoryRegion)
                {
                    if (this.Configuration.DisplayTerritoryName || this.Configuration.DisplayPlaceName)
                    {
                        locationString = $"{locationString}{separator2} {this.territoryRegion}";
                    } else
                    {
                        locationString = this.territoryRegion;
                    }
                }

                UpdateDtrBarEntry($"{leadingText}{locationString}{tailingText}");
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Could not run OnUpdate.");
            }
        }

        private void UpdateDtrBarEntry(string text = "")
        {
            dtrEntry.Text = text;
        }

        private void OnCommand(string command, string args)
        {
            this.PluginUi.SettingsVisible = true;
        }

        private void DrawUI()
        {
            this.PluginUi.Draw();
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
