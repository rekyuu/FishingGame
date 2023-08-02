using System.Collections.Generic;
using FishingGame.Data;
using FishingGame.Utilities;
using Godot;

namespace FishingGame.Managers;

public partial class DataManager : Node
{
    public static Dictionary<string, FishZoneData[]> FishZones { get; private set; }

    public override void _Ready()
    {
        FishZones = Utility.LoadFromJson<Dictionary<string, FishZoneData[]>>("res://data/fish_zones.json");
    }
}