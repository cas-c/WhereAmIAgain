using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dalamud.Game;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Housing;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;

namespace WhereAmIAgain;

public unsafe partial class DtrDisplay : IDisposable
{
    private static Configuration Config => Service.Configuration;

    private PlaceName? currentContinent;
    private PlaceName? currentTerritory;
    private PlaceName? currentRegion;
    private PlaceName? currentSubArea;
    private string? currentWard;

    private static TerritoryInfo* AreaInfo => TerritoryInfo.Instance();
    private static HousingManager* HousingInfo => HousingManager.Instance();

    private uint lastTerritory;
    private uint lastRegion;
    private uint lastSubArea;
    private sbyte lastHousingWard;
    private short lastHousingRoom;
    private sbyte lastHousingPlot;
    private byte lastHousingDivision;

    private readonly DtrBarEntry dtrEntry;

    private bool locationChanged;
    
    [GeneratedRegex("(?<={\\p{N}})")]
    private static partial Regex SubstringSplitRegex();
    
    [GeneratedRegex("[^\\p{L}\\p{N}]*$")]
    private static partial Regex DoesNotEndWithAlphanumericRegex();
    
    [GeneratedRegex("^[^\\p{L}\\p{N}]*|")]
    private static partial Regex DoesNotStartWithAlphanumericRegex();
    
    public DtrDisplay()
    {
        SignatureHelper.Initialise(this);
        
        dtrEntry = Service.DtrBar.Get("Where am I again?");
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.TerritoryChanged += OnZoneChange;
    }

    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.TerritoryChanged -= OnZoneChange;

        dtrEntry.Dispose();
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (Service.ClientState.LocalPlayer is null) return;

        UpdateRegion();
        UpdateSubArea();
        UpdateTerritory();

        if (Service.Configuration.UsePreciseHousingLocation)
        {
            UpdatePreciseHousing();
        }
        else
        {
            UpdateHousing();
        }
        
        if (locationChanged)
        {
            UpdateDtrText();
        }
    }

    private void OnZoneChange(object? sender, ushort e) => locationChanged = true;

    public void UpdateDtrText()
    {
        var preTextEnd = Config.FormatString.IndexOf('{');
        var postTextStart = Config.FormatString.LastIndexOf('}') + 1;
        var formattedString = Config.FormatString;
        
        try
        {
            // Split the string into individual format specifiers
            
            // Example List:
            // {0}
            // , {1}
            // , {2}
            // , {3}
            var substrings = SubstringSplitRegex().Split(formattedString[preTextEnd..postTextStart]);

            // Get a string with intermediary symbols removed
            var internalString = substrings
                    
                    // Fill each substring with the correct format data
                .Select(str => str.Format(
                    currentContinent?.Name.RawString ?? string.Empty, 
                    currentTerritory?.Name.RawString ?? string.Empty, 
                    currentRegion?.Name.RawString ?? string.Empty, 
                    currentSubArea?.Name.RawString ?? string.Empty, 
                    currentWard ?? string.Empty))
                
                    // Starting at the end of each substring, work backwards and replace all non-alphanumeric symbols with empty
                .Select(substring => DoesNotEndWithAlphanumericRegex().Replace(substring, string.Empty))
                    
                    // Append all of the strings together, entries that were entirely separators were replaced with string.Empty
                .Aggregate(string.Empty, (current, newStr) => current + newStr);

            // Strip non-alphanumeric characters from the start of the internal string
            internalString = DoesNotStartWithAlphanumericRegex().Replace(internalString, string.Empty);
            
            if (Config.ShowInstanceNumber)
            {
                internalString += GetCharacterForInstanceNumber(UIState.Instance()->AreaInstance.Instance);
            }

            formattedString = Config.FormatString[..preTextEnd] + internalString + Config.FormatString[postTextStart..];
        }
        catch(FormatException)
        {
            // Ignore format exceptions, because we warn the user in the config window if they are missing a format symbol 
        }
        catch(ArgumentOutOfRangeException)
        {
            // Ignore range exceptions
        }
        
        dtrEntry.Text = new SeStringBuilder().AddText(formattedString).BuiltString;
        locationChanged = false;
    }

    private static string GetCharacterForInstanceNumber(int instance)
    {
        if (instance == 0) return string.Empty;
        
        return $" {((SeIconChar)((int)SeIconChar.Instance1 + (instance - 1))).ToIconChar()}";
    }

    private void UpdateTerritory()
    {
        if (lastTerritory != Service.ClientState.TerritoryType)
        {
            lastTerritory = Service.ClientState.TerritoryType;
            var territory = Service.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(Service.ClientState.TerritoryType);

            currentTerritory = territory?.PlaceName.Value;
            currentContinent = territory?.PlaceNameRegion.Value;
            locationChanged = true;
        }
    }

    private void UpdateSubArea()
    {
        if (lastSubArea != AreaInfo->SubAreaPlaceNameID)
        {
            lastSubArea = AreaInfo->SubAreaPlaceNameID;
            currentSubArea = GetPlaceName(AreaInfo->SubAreaPlaceNameID);
            locationChanged = true;
        }
    }

    private void UpdateRegion()
    {
        if (lastRegion != AreaInfo->AreaPlaceNameID)
        {
            lastRegion = AreaInfo->AreaPlaceNameID;
            currentRegion = GetPlaceName(AreaInfo->AreaPlaceNameID);
            locationChanged = true;
        }
    }
    
    private void UpdateHousing()
    {
        if (HousingInfo is null || HousingInfo->CurrentTerritory is null)
        {
            currentWard = null;
            return;
        }

        var ward = (sbyte) (HousingInfo->GetCurrentWard() + 1);

        if (lastHousingWard != ward)
        {
            lastHousingWard = ward;
            currentWard = $"Ward {ward}";
            locationChanged = true;
        }
    }
    
    private void UpdatePreciseHousing()
    {
        if (HousingInfo is null)
        {
            currentWard = null;
            return;
        }

        var ward = HousingInfo->GetCurrentWard();
        var room = HousingInfo->GetCurrentRoom();
        var plot = HousingInfo->GetCurrentPlot();
        var division = HousingInfo->GetCurrentDivision();

        if (ward != lastHousingWard || room != lastHousingRoom || plot != lastHousingPlot || division != lastHousingDivision)
        {
            lastHousingWard = ward;
            lastHousingRoom = room;
            lastHousingPlot = plot;
            lastHousingDivision = division;
            currentWard = GetCurrentHouseAddress();
            locationChanged = true;
        }
    }
    
    public string GetCurrentHouseAddress() 
    {
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

        switch (plot) 
        {
            case < -1:
                strings.Add($"Apartment {(room == 0 ? $"Lobby" : $"{room}")}");
                break;
            
            case > -1: 
                strings.Add($"Plot {plot+1}");
                if (room > 0) {
                    strings.Add($"Room {room}");
                }
                break;
        }

        return string.Join(" ", strings);
    }

    private static PlaceName? GetPlaceName(uint row) => Service.DataManager.GetExcelSheet<PlaceName>()!.GetRow(row);
}
