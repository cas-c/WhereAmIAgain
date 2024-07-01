using Dalamud.Interface.Windowing;

namespace WhereAmIAgain;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class System {
	public static WindowSystem WindowSystem { get; set; }
	public static Configuration Config { get; set; }
	public static ConfigurationWindow ConfigurationWindow { get; set; }
	public static DtrDisplay DtrDisplay { get; set; }
}