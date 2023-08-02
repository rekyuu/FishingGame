using System.Collections.Generic;
using System.Linq;
using FishingGame.Data;
using FishingGame.Managers;
using FishingGame.Utilities;
using Godot;

namespace FishingGame;

public partial class FishZoneController : Node3D
{
    [Export] public string FishZoneDataName = "none";

    private readonly WeightedPool<Resources.Fish> _fishPool = new();
    private Resources.Fish _nullFish;
    
    public override void _Ready()
    {
        _nullFish = GD.Load<Resources.Fish>("res://resources/fish/null_fish.tres");
        
        FishZoneData[] data = DataManager.FishZones[FishZoneDataName];
        foreach (FishZoneData zoneData in data)
        {
            WeightedElement<Resources.Fish> element = new()
            {
                Resource = GD.Load<Resources.Fish>($"res://resources/fish/{zoneData.FishName}.tres"),
                Weight = zoneData.FishWeight
            };

            _fishPool.Elements.Add(element);
        }
    }

    public Resources.Fish GetNextFish()
    {
        List<WeightedElement<Resources.Fish>> availableFish = _fishPool.Elements
            .Where(x => x.Resource.AvailableSeason == GameManager.Instance.Season)
            .Where(x => FishIsInAvailableTimeframe(x.Resource.AvailableStartHour, x.Resource.AvailableForHours, GameManager.Instance.TimeOfDay))
            .ToList();

        if (availableFish.Count == 0) return _nullFish;

        WeightedPool<Resources.Fish> updatedFishPool = new() { Elements = availableFish };
        updatedFishPool.UpdateWeights();

        return updatedFishPool.GetRandomElement();
    }

    public static bool FishIsInAvailableTimeframe(int availableStartHour, int availableForHours, float currentTime)
    {
        int endHour = availableStartHour + availableForHours;
        bool isAfterStartTime = availableStartHour / 24f <= currentTime;
        bool isBeforeEndTime = currentTime < endHour / 24f;

        if (endHour >= 24 && currentTime < availableStartHour)
        {
            int endHourCanonical = endHour - 24;
            isAfterStartTime = true;
            isBeforeEndTime = currentTime < endHourCanonical / 24f;
        }

        return isAfterStartTime && isBeforeEndTime;
    }
}