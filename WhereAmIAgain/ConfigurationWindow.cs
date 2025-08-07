using System.Linq;
using System.Numerics;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace WhereAmIAgain;

public class ConfigurationWindow : Window {
	public ConfigurationWindow() : base("Where am I again? - Settings") {
		Flags |= ImGuiWindowFlags.AlwaysAutoResize;
	}

	public override void Draw() {
		ImGui.Text("Use the text box below to define how you want the text to be formatted.\n" +
		           "Use symbols {0} {1} {2} {3} {4} where you want the following values to be in the string\n\n" +
		           "{0} - Region (Ex. The Northern Empty)\n" +
		           "{1} - Territory (Ex. Old Sharlayan)\n" +
		           "{2} - Area (Ex. Archons Design)\n" +
		           "{3} - Sub-Area (Ex. Old Sharlayan Aetheryte Plaza)\n" +
		           "{4} - Housing Ward (Ex. Ward 14)");

		ImGuiHelpers.ScaledDummy(10.0f);

		FormatInputBox("Info Bar Entry", ref System.Config.FormatString);
		FormatInputBox("Info Bar Tooltip", ref System.Config.TooltipFormatString);

		ImGuiHelpers.ScaledDummy(10.0f);

		if (ImGui.Checkbox("Show Instance Number", ref System.Config.ShowInstanceNumber)) {
			System.Config.Save();
		}
		ImGuiComponents.HelpMarker("Shows the instance number for the current instance at the end of the string");

		if (ImGui.Checkbox("Show Precise Housing Location", ref System.Config.UsePreciseHousingLocation)) {
			System.Config.Save();
		}
		ImGuiComponents.HelpMarker("Replaces 'Ward 14' with `Ward 14 Subdivision Plot 23`");
	}

	private static void FormatInputBox(string label, ref string setting) {
		if (ImGui.BeginTable("FormatEntryTable", 3)) {
			ImGui.TableSetupColumn("##Label", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("##Input", ImGuiTableColumnFlags.WidthFixed, 300.0f * ImGuiHelpers.GlobalScale);
			ImGui.TableSetupColumn("##ResetButton", ImGuiTableColumnFlags.WidthStretch);

			ImGui.TableNextColumn();
			ImGui.Text(label);

			ImGui.TableNextColumn();
			ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
			if (ImGui.InputText($"##InputString{label}", ref setting, 2047)) {
				System.DtrDisplay.UpdateDtrText();
			}

			if (ImGui.IsItemDeactivatedAfterEdit()) {
				if (!BracesMismatched(setting)) {
					System.Config.Save();
				}
			}

			ImGui.TableNextColumn();
			if (ImGui.Button($"Reset To Default##{label}")) {
				setting = "{0}, {1}, {2}, {3}";
				System.DtrDisplay.UpdateDtrText();
				System.Config.Save();
			}

			ImGui.EndTable();
		}

		if (BracesMismatched(setting)) {
			ImGui.TextColored(new Vector4(1.0f, 0.2f, 0.2f, 1.0f), $"There is an error in the {label} format string.\nPlease check there is a open brace {{ and a matching close brace }}");
		}
	}

	private static bool BracesMismatched(string formatString)
		=> formatString.Count(c => c == '{') != formatString.Count(c => c == '}');
}