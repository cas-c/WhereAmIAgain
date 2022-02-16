using ImGuiNET;
using System;
using System.Numerics;

namespace WhereAmIAgain
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Plugin Plugin;
        private Configuration Configuration;
        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }

        // passing in the image here just for simplicity
        public PluginUI(Plugin Plugin)
        {
            this.Plugin = Plugin;
            this.Configuration = Plugin.Configuration;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            DrawSettingsWindow();
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(600, 400), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Where am I again? Settings", ref this.settingsVisible))
            {
                ImGui.Text($"Current Text: {this.Plugin.LastUpdatedText}");
                ImGui.Text("");
                var DisplayZoneLead = this.Configuration.DisplayZoneLead;
                if (ImGui.Checkbox("Add text before place name", ref DisplayZoneLead))
                {
                    this.Configuration.DisplayZoneLead = DisplayZoneLead;
                    this.Configuration.Save();
                }

                if (DisplayZoneLead)
                {
                    var DisplayZoneLeadingText = this.Configuration.DisplayZoneLeadingText;
                    ImGui.InputText("##DisplayZoneLeadingText", ref DisplayZoneLeadingText, 50);
                    if (this.Configuration.DisplayZoneLeadingText != DisplayZoneLeadingText)
                    {
                        this.Configuration.DisplayZoneLeadingText = DisplayZoneLeadingText;
                        this.Configuration.Save();
                    }
                }

                var DisplayPlaceName = this.Configuration.DisplayPlaceName;
                var stringPlaceName = "The Octant";
                if (this.Plugin.playerZone != "")
                {
                    stringPlaceName = this.Plugin.playerZone;
                }

                if (ImGui.Checkbox($"Display Place Name (e.g. {stringPlaceName})", ref DisplayPlaceName))
                {
                    this.Configuration.DisplayPlaceName = DisplayPlaceName;
                    this.Configuration.Save();
                }

                var DisplayZoneSeparator = this.Configuration.DisplayZoneSeparator;
                if (ImGui.Checkbox("Display a separator between Place and Territory ", ref DisplayZoneSeparator))
                {
                    this.Configuration.DisplayZoneSeparator = DisplayZoneSeparator;
                    this.Configuration.Save();
                }

                if (DisplayZoneSeparator)
                {
                    var ZoneSeparator = this.Configuration.ZoneSeparator;
                    ImGui.InputText("##ZoneSeparator", ref ZoneSeparator, 50);
                    if (this.Configuration.ZoneSeparator != ZoneSeparator)
                    {
                        this.Configuration.ZoneSeparator = ZoneSeparator;
                        this.Configuration.Save();
                    }
                }

                var DisplayTerritoryName = this.Configuration.DisplayTerritoryName;
                if (ImGui.Checkbox($"Display Territory Name (e.g. {this.Plugin.territoryName ?? "Limsa Lominsa Lower Decks"})", ref DisplayTerritoryName))
                {
                    this.Configuration.DisplayTerritoryName = DisplayTerritoryName;
                    this.Configuration.Save();
                }

                var DisplayZoneSeparator2 = this.Configuration.DisplayZoneSeparator2;
                if (ImGui.Checkbox("Display a separator between Territory and Region ", ref DisplayZoneSeparator2))
                {
                    this.Configuration.DisplayZoneSeparator2 = DisplayZoneSeparator2;
                    this.Configuration.Save();
                }

                if (DisplayZoneSeparator2)
                {
                    var ZoneSeparator2 = this.Configuration.ZoneSeparator2;
                    ImGui.InputText("##ZoneSeparator2", ref ZoneSeparator2, 50);
                    if (this.Configuration.ZoneSeparator2 != ZoneSeparator2)
                    {
                        this.Configuration.ZoneSeparator2 = ZoneSeparator2;
                        this.Configuration.Save();
                    }
                }

                var DisplayTerritoryRegion = this.Configuration.DisplayTerritoryRegion;
                if (ImGui.Checkbox($"Display Territory Region (e.g. {this.Plugin.territoryRegion ?? "La Noscea"})", ref DisplayTerritoryRegion))
                {
                    this.Configuration.DisplayTerritoryRegion = DisplayTerritoryRegion;
                    this.Configuration.Save();
                }

                var DisplayZoneTailing = this.Configuration.DisplayZoneTailing;
                if (ImGui.Checkbox("Add text after region name ", ref DisplayZoneTailing))
                {
                    this.Configuration.DisplayZoneTailing = DisplayZoneTailing;
                    this.Configuration.Save();
                }

                if (DisplayZoneTailing)
                {
                    var DisplayZoneTailingText = this.Configuration.DisplayZoneTailingText;
                    ImGui.InputText("##DisplayZoneTailingText", ref DisplayZoneTailingText, 50);
                    if (this.Configuration.DisplayZoneTailingText != DisplayZoneTailingText)
                    {
                        this.Configuration.DisplayZoneTailingText = DisplayZoneTailingText;
                        this.Configuration.Save();
                    }
                }

                ImGui.Text(" ");
                ImGui.Text("~ additional settings ~");

                var RemoveDuplicates = this.Configuration.RemoveDuplicates;
                if (ImGui.Checkbox("Try to filter out duplicates in zone names", ref RemoveDuplicates))
                {
                    this.Configuration.RemoveDuplicates = RemoveDuplicates;
                    this.Configuration.Save();
                }

                ImGui.Text("Removes repeated zone names (such as in Housing districts, Aetheryte plazas)");

            }
            ImGui.End();
        }
    }
}
