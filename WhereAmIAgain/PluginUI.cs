using ImGuiNET;
using System;
using System.Numerics;

namespace WhereAmIAgain
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;

        public string territoryName;
        public string territoryRegion;
        public string playerZone;
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
        public PluginUI(Configuration configuration)
        {
            this.configuration = configuration;
            this.territoryName = "initial name";
            this.territoryRegion = "initial region";
        }

        public void Dispose()
        {
        }

        public void Draw(string territoryName, string territoryRegion, string playerZone)
        {
            this.territoryName = territoryName;
            this.territoryRegion = territoryRegion;
            this.playerZone = playerZone;
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
                var DisplayZoneLead = this.configuration.DisplayZoneLead;
                if (ImGui.Checkbox("Add text before place name", ref DisplayZoneLead))
                {
                    this.configuration.DisplayZoneLead = DisplayZoneLead;
                    this.configuration.Save();
                }

                if (DisplayZoneLead)
                {
                    var DisplayZoneLeadingText = this.configuration.DisplayZoneLeadingText;
                    ImGui.InputText("##DisplayZoneLeadingText", ref DisplayZoneLeadingText, 50);
                    if (this.configuration.DisplayZoneLeadingText != DisplayZoneLeadingText)
                    {
                        this.configuration.DisplayZoneLeadingText = DisplayZoneLeadingText;
                        this.configuration.Save();
                    }
                }

                var DisplayPlaceName = this.configuration.DisplayPlaceName;
                var stringPlaceName = "The Octant";
                if (this.playerZone != "")
                {
                    stringPlaceName = this.playerZone;
                }

                if (ImGui.Checkbox($"Display Place Name (e.g. {stringPlaceName})", ref DisplayPlaceName))
                {
                    this.configuration.DisplayPlaceName = DisplayPlaceName;
                    this.configuration.Save();
                }

                var DisplayZoneSeparator = this.configuration.DisplayZoneSeparator;
                if (ImGui.Checkbox("Display a separator between Place and Territory ", ref DisplayZoneSeparator))
                {
                    this.configuration.DisplayZoneSeparator = DisplayZoneSeparator;
                    this.configuration.Save();
                }

                if (DisplayZoneSeparator)
                {
                    var ZoneSeparator = this.configuration.ZoneSeparator;
                    ImGui.InputText("##ZoneSeparator", ref ZoneSeparator, 50);
                    if (this.configuration.ZoneSeparator != ZoneSeparator)
                    {
                        this.configuration.ZoneSeparator = ZoneSeparator;
                        this.configuration.Save();
                    }
                }

                var DisplayTerritoryName = this.configuration.DisplayTerritoryName;
                if (ImGui.Checkbox($"Display Territory Name (e.g. {this.territoryName ?? "Limsa Lominsa Lower Decks"})", ref DisplayTerritoryName))
                {
                    this.configuration.DisplayTerritoryName = DisplayTerritoryName;
                    this.configuration.Save();
                }

                var DisplayZoneSeparator2 = this.configuration.DisplayZoneSeparator2;
                if (ImGui.Checkbox("Display a separator between Territory and Region ", ref DisplayZoneSeparator2))
                {
                    this.configuration.DisplayZoneSeparator2 = DisplayZoneSeparator2;
                    this.configuration.Save();
                }

                if (DisplayZoneSeparator2)
                {
                    var ZoneSeparator2 = this.configuration.ZoneSeparator2;
                    ImGui.InputText("##ZoneSeparator2", ref ZoneSeparator2, 50);
                    if (this.configuration.ZoneSeparator2 != ZoneSeparator2)
                    {
                        this.configuration.ZoneSeparator2 = ZoneSeparator2;
                        this.configuration.Save();
                    }
                }

                var DisplayTerritoryRegion = this.configuration.DisplayTerritoryRegion;
                if (ImGui.Checkbox($"Display Territory Region (e.g. {this.territoryRegion ?? "La Noscea"})", ref DisplayTerritoryRegion))
                {
                    this.configuration.DisplayTerritoryRegion = DisplayTerritoryRegion;
                    this.configuration.Save();
                }

                var DisplayZoneTailing = this.configuration.DisplayZoneTailing;
                if (ImGui.Checkbox("Add text after region name ", ref DisplayZoneTailing))
                {
                    this.configuration.DisplayZoneTailing = DisplayZoneTailing;
                    this.configuration.Save();
                }

                if (DisplayZoneTailing)
                {
                    var DisplayZoneTailingText = this.configuration.DisplayZoneTailingText;
                    ImGui.InputText("##DisplayZoneTailingText", ref DisplayZoneTailingText, 50);
                    if (this.configuration.DisplayZoneTailingText != DisplayZoneTailingText)
                    {
                        this.configuration.DisplayZoneTailingText = DisplayZoneTailingText;
                        this.configuration.Save();
                    }
                }

                ImGui.Text(" ");
                ImGui.Text("~ additional settings ~");

                var RemoveDuplicates = this.configuration.RemoveDuplicates;
                if (ImGui.Checkbox("Try to filter out duplicates in zone names", ref RemoveDuplicates))
                {
                    this.configuration.RemoveDuplicates = RemoveDuplicates;
                    this.configuration.Save();
                }

                ImGui.Text("Removes repeated zone names (such as in Housing districts, Aetheryte plazas)");

            }
            ImGui.End();
        }
    }
}
