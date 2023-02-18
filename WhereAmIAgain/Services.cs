using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Interface.GameFonts;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace WhereAmIAgain;

public class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static DataManager DataManager { get; private set; } = null!;
    [PluginService] public static DtrBar DtrBar { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;
    [PluginService] public static CommandManager CommandManager { get; private set; } = null!;
    
    public static Configuration Configuration { get; set; } = null!;
    public static ConfigurationWindow ConfigurationWindow { get; set; } = null!;
    public static DtrDisplay DtrDisplay { get; set; } = null!;
    public static GameFontHandle AxisFont { get; set; } = null!;

}