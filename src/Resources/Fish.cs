using FishingGame.Utilities;
using Godot;

namespace FishingGame.Resources;

public partial class Fish : Resource
{
    [Export]
    public string Name { get; set; }
    
    [Export]
    public int MinWaitTimeSecs { get; set; }
    
    [Export]
    public int MaxWaitTimeSecs { get; set; }
    
    [Export]
    public float MinSizeCm { get; set; }
    
    [Export]
    public float MaxSizeCm { get; set; }
    
    [Export]
    public int AvailableStartHour { get; set; }
    
    [Export]
    public int AvailableForHours { get; set; }
    
    [Export]
    public int AvailableSeason { get; set; }
    
    [Export]
    public int AvailableWeather { get; set; }
    
    [Export]
    public int RequiredHookset { get; set; }
    
    [Export]
    public int[] PreferredBait { get; set; }
    
    [Export]
    public float GetAwayChance { get; set; }

    public float GetTimeToBite() => Utility.GetNextFloat(MinWaitTimeSecs, MaxWaitTimeSecs);
    
    public int GetSize() => Mathf.RoundToInt(Utility.GetNextFloat(MinSizeCm, MaxSizeCm));
}