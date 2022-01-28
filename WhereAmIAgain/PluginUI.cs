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

        public void Draw(string territoryName, string territoryRegion)
        {
            this.territoryName = territoryName;
            this.territoryRegion = territoryRegion;
            DrawSettingsWindow();
        }


        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(400, 75), ImGuiCond.Always);
            if (ImGui.Begin("Where am I again? Settings", ref this.settingsVisible,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                // // can't ref a property, so use a local copy
                var configValue = this.configuration.DisplayTerritoryRegion;
                if (ImGui.Checkbox("Display Territory Region (e.g. Gridania)", ref configValue))
                {
                     this.configuration.DisplayTerritoryRegion = configValue;
                     this.configuration.Save();
                }
            }
            ImGui.End();
        }
    }
}
