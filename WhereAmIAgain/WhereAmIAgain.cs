using Dalamud.Game.Command;
using Dalamud.Plugin;

namespace WhereAmIAgain;

public sealed class WhereAmIAgainPlugin : IDalamudPlugin {
	private const string CommandName = "/waia";

	public WhereAmIAgainPlugin(IDalamudPluginInterface pluginInterface) {
		pluginInterface.Create<Service>();

		System.Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

		Service.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
			HelpMessage = "Open configuration window",
		});

		System.DtrDisplay = new DtrDisplay();

		System.WindowSystem = new("Where am I again?");
		System.ConfigurationWindow = new ConfigurationWindow();
		System.WindowSystem.AddWindow(System.ConfigurationWindow);

		Service.PluginInterface.UiBuilder.Draw += DrawUi;
		Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUi;
	}

	public void Dispose() {
		Service.PluginInterface.UiBuilder.Draw -= DrawUi;
		Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUi;

		System.WindowSystem.RemoveAllWindows();
		Service.CommandManager.RemoveHandler(CommandName);

		System.DtrDisplay.Dispose();
	}

	private void OnCommand(string command, string arguments)
		=> System.ConfigurationWindow.Toggle();

	private void DrawUi()
		=> System.WindowSystem.Draw();

	private void DrawConfigUi()
		=> System.ConfigurationWindow.Toggle();
}