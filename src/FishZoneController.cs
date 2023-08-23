using System.Collections.Generic;
using System.Linq;
using FishingGame.Data;
using FishingGame.Managers;
using FishingGame.Resources;
using FishingGame.Utilities;
using Godot;

namespace FishingGame;

public partial class FishZoneController : Node3D
{
    [Export] public string FishZoneDataName = "none";

    private readonly List<WeightedElement<Fish>> _fish = new();
    private Fish _nullFish;
    
    public override void _Ready()
    {
        _nullFish = GD.Load<Fish>("res://resources/fish/null_fish.tres");
        
        FishZoneData[] data = DataManager.FishZones[FishZoneDataName];
        foreach (FishZoneData zoneData in data)
        {
            WeightedElement<Fish> element = new()
            {
                Resource = GD.Load<Fish>($"res://resources/fish/{zoneData.FishName}.tres"),
                Weight = zoneData.FishWeight
            };

            _fish.Add(element);
        }
    }

    public WeightedPool<Fish> GetAvailableFishPool()
    {
        List<WeightedElement<Fish>> availableFish = GetAvailableFish();
        WeightedPool<Fish> fishPool = new() { Elements = availableFish };
        fishPool.UpdateWeights();

        return fishPool;
    }

    public Fish GetNextFish()
    {
        WeightedPool<Fish> fishPool = GetAvailableFishPool();
        return fishPool.Elements.Count == 0 ? _nullFish : fishPool.GetRandomElement();
    }

    public static bool FishIsInAvailableTimeframe(int availableStartHour, int availableForHours, float currentTime)
    {
        int endHour = availableStartHour + availableForHours;
        bool isAfterStartTime = availableStartHour / 24f <= currentTime;
        bool isBeforeEndTime = currentTime < endHour / 24f;

        if (endHour >= 24 && currentTime < availableStartHour / 24f)
        {
            int endHourCanonical = endHour - 24;
            isAfterStartTime = true;
            isBeforeEndTime = currentTime < endHourCanonical / 24f;
        }

        return isAfterStartTime && isBeforeEndTime;
    }

    private List<WeightedElement<Fish>> GetAvailableFish()
    {
        return _fish
            .Where(x => x.Resource.AvailableSeason == Season.Any || x.Resource.AvailableSeason == GameManager.Instance.Season)
            .Where(x => x.Resource.AvailableWeather == Weather.Any || x.Resource.AvailableWeather == GameManager.Instance.Weather)
            .Where(x => FishIsInAvailableTimeframe(x.Resource.AvailableStartHour, x.Resource.AvailableForHours, GameManager.Instance.TimeOfDay))
            .ToList();
    }
}