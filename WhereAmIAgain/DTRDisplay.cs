using System;
using System.Collections.Generic;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using TerritoryType = Lumina.Excel.GeneratedSheets.TerritoryType;

namespace WhereAmIAgain;

public unsafe class DtrDisplay : IDisposable {

	private readonly IDtrBarEntry dtrEntry;
	private PlaceName? currentContinent;
	private PlaceName? currentRegion;
	private PlaceName? currentSubArea;
	private PlaceName? currentTerritory;
	private string? currentWard;
	private byte lastHousingDivision;
	private sbyte lastHousingPlot;
	private short lastHousingRoom;
	private sbyte lastHousingWard;
	private uint lastRegion;
	private uint lastSubArea;

	private uint lastTerritory;

	private bool locationChanged;

	public DtrDisplay() {
		dtrEntry = Service.DtrBar.Get("Where am I again?");

		dtrEntry.OnClick += () => {
			System.ConfigurationWindow.Toggle();
		};

		Service.Framework.Update += OnFrameworkUpdate;
		Service.ClientState.TerritoryChanged += OnZoneChange;
	}

	private static TerritoryInfo* AreaInfo => TerritoryInfo.Instance();
	private static HousingManager* HousingInfo => HousingManager.Instance();

	public void Dispose() {
		Service.Framework.Update -= OnFrameworkUpdate;
		Service.ClientState.TerritoryChanged -= OnZoneChange;

		dtrEntry.OnClick = null;
		dtrEntry.Remove();
	}

	private void OnFrameworkUpdate(IFramework framework) {
		if (Service.ClientState.LocalPlayer is null) return;

		UpdateRegion();
		UpdateSubArea();
		UpdateTerritory();

		if (System.Config.UsePreciseHousingLocation) {
			UpdatePreciseHousing();
		}
		else {
			UpdateHousing();
		}

		if (locationChanged) {
			UpdateDtrText();
		}
	}

	private void OnZoneChange(ushort e)
		=> locationChanged = true;

	public void UpdateDtrText() {
		var dtrString = FormatString(System.Config.FormatString);
		var tooltipString = FormatString(System.Config.TooltipFormatString);

		dtrEntry.Text = dtrString;
		dtrEntry.Tooltip = tooltipString.Replace(@"\n", "\n");
		locationChanged = false;
	}

	private string GetStringForIndex(int index) => index switch {
		0 => currentContinent?.Name.RawString ?? string.Empty,
		1 => currentTerritory?.Name.RawString ?? string.Empty,
		2 => currentRegion?.Name.RawString ?? string.Empty,
		3 => currentSubArea?.Name.RawString ?? string.Empty,
		4 => currentWard ?? string.Empty,
		_ => string.Empty,
	};

	private string FormatString(string inputFormat) {
		try {
			var preTextEnd = inputFormat.IndexOf('{');
			var postTextStart = inputFormat.LastIndexOf('}') + 1;
			var workingSegment = inputFormat[preTextEnd..postTextStart];

			// Get all the segments and the text before them
			// If the segment itself resolves to an empty modifier, we omit the preceding text.
			var splits = workingSegment.Split('}');
			var internalString = string.Empty;
			foreach (var segment in splits) {
				if (segment.IsNullOrEmpty()) continue;

				var separator = segment[..^2];
				var location = GetStringForIndex(int.Parse(segment[^1..]));

				if (location.IsNullOrEmpty()) continue;
				internalString += internalString == string.Empty ? $"{location}" : $"{separator}{location}";
			}

			if (System.Config.ShowInstanceNumber) {
				internalString += GetCharacterForInstanceNumber(UIState.Instance()->PublicInstance.InstanceId);
			}

			return inputFormat[..preTextEnd] + internalString + inputFormat[postTextStart..];
		}
		catch (Exception) {
			// ignored
		}

		return string.Empty;
	}

	private static string GetCharacterForInstanceNumber(uint instance) {
		if (instance == 0) return string.Empty;

		return $" {((SeIconChar) ((int) SeIconChar.Instance1 + (instance - 1))).ToIconChar()}";
	}

	private void UpdateTerritory() {
		if (lastTerritory != Service.ClientState.TerritoryType) {
			lastTerritory = Service.ClientState.TerritoryType;
			var territory = Service.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(Service.ClientState.TerritoryType);

			currentTerritory = territory?.PlaceName.Value;
			currentContinent = territory?.PlaceNameRegion.Value;
			locationChanged = true;
		}
	}

	private void UpdateSubArea() {
		if (lastSubArea != AreaInfo->SubAreaPlaceNameId) {
			lastSubArea = AreaInfo->SubAreaPlaceNameId;
			currentSubArea = GetPlaceName(AreaInfo->SubAreaPlaceNameId);
			locationChanged = true;
		}
	}

	private void UpdateRegion() {
		if (lastRegion != AreaInfo->SubAreaPlaceNameId) {
			lastRegion = AreaInfo->SubAreaPlaceNameId;
			currentRegion = GetPlaceName(AreaInfo->SubAreaPlaceNameId);
			locationChanged = true;
		}
	}

	private void UpdateHousing() {
		if (HousingInfo is null || HousingInfo->CurrentTerritory is null) {
			currentWard = null;
			return;
		}

		var ward = (sbyte) (HousingInfo->GetCurrentWard() + 1);

		if (lastHousingWard != ward) {
			lastHousingWard = ward;
			currentWard = $"Ward {ward}";
			locationChanged = true;
		}
	}

	private void UpdatePreciseHousing() {
		if (HousingInfo is null) {
			currentWard = null;
			return;
		}

		var ward = HousingInfo->GetCurrentWard();
		var room = HousingInfo->GetCurrentRoom();
		var plot = HousingInfo->GetCurrentPlot();
		var division = HousingInfo->GetCurrentDivision();

		if (ward != lastHousingWard || room != lastHousingRoom || plot != lastHousingPlot || division != lastHousingDivision) {
			lastHousingWard = ward;
			lastHousingRoom = room;
			lastHousingPlot = plot;
			lastHousingDivision = division;
			currentWard = GetCurrentHouseAddress();
			locationChanged = true;
		}
	}

	private string GetCurrentHouseAddress() {
		var housingManager = HousingManager.Instance();
		if (housingManager == null) return string.Empty;
		var strings = new List<string>();

		var ward = housingManager->GetCurrentWard() + 1;
		if (ward == 0) return string.Empty;

		var plot = housingManager->GetCurrentPlot();
		var room = housingManager->GetCurrentRoom();
		var division = housingManager->GetCurrentDivision();

		strings.Add($"Ward {ward}");
		if (division == 2 || plot is >= 30 or -127) strings.Add($"Subdivision");

		switch (plot) {
			case < -1:
				strings.Add($"Apartment {(room == 0 ? $"Lobby" : $"{room}")}");
				break;

			case > -1:
				strings.Add($"Plot {plot + 1}");
				if (room > 0) {
					strings.Add($"Room {room}");
				}
				break;
		}

		return string.Join(" ", strings);
	}

	private static PlaceName? GetPlaceName(uint row)
		=> Service.DataManager.GetExcelSheet<PlaceName>()!.GetRow(row);
}