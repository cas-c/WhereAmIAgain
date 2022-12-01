using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace WhereAmIAgain;

public class ConfigurationWindow : Window, IDisposable
{
    private readonly WindowSystem windowSystem = new("Where am I again?");
    private Configuration Configuration => Service.Configuration;
    
    private const string CommandName = "/waia";
    
    public ConfigurationWindow() : base("Where am I again? - Settings")
    {
        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        
        Service.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Displays the local in-game zone next to your server info."
        });

        Flags |= ImGuiWindowFlags.AlwaysAutoResize;
        
        windowSystem.AddWindow(this);
    }

    private void OnCommand(string command, string arguments) => IsOpen = !IsOpen;
    private void DrawUI() => windowSystem.Draw();
    private void DrawConfigUI() => IsOpen = true;

    public override void Draw()
    {
        ImGui.Text("Use the text box below to define how you want the text to be formatted.\n" +
                   "Use symbols {0} {1} {2} {3} where you want the following values to be in the string\n\n" +
                   "{0} - Region (Ex. The Northern Empty)\n" +
                   "{1} - Territory (Ex. Old Sharlayan)\n" +
                   "{2} - Area (Ex. Archon's Design)\n" +
                   "{3} - Sub-Area (Ex. Old Sharlayan Aetheryte Plaza)");
        
        ImGuiHelpers.ScaledDummy(10.0f);

        ImGui.InputText("##InputString", ref Configuration.FormatString, 35);
        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            if (!BracesMismatched())
            {
                Service.Configuration.Save();
            }
        }
        
        ImGui.SameLine();
        if (ImGui.Button("Reset To Default"))
        {
            Configuration.FormatString = "{0}, {1}, {2}, {3}";
            Service.Configuration.Save();
        }

        if (BracesMismatched())
        {
            ImGui.TextColored(new Vector4(1.0f, 0.2f, 0.2f, 1.0f), "There is an error in the format string.\nPlease check there is a open brace { and a matching close brace }");
        }
    }

    private bool BracesMismatched()
    {
        return Configuration.FormatString.Count(c => c == '{') != Configuration.FormatString.Count(c => c == '}');
    }
    
    public void Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
        
        windowSystem.RemoveAllWindows();
        Service.CommandManager.RemoveHandler(CommandName);
    }
}