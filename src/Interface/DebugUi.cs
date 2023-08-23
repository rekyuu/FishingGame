using System;
using System.Collections.Generic;
using System.Linq;
using FishingGame.Managers;
using FishingGame.Resources;
using FishingGame.Utilities;
using Godot;

namespace FishingGame.Interface;

public partial class DebugUi : Control
{
    [ExportSubgroup("Time Container")]
    [Export] public Label TimeLabel;
    [Export] public Label DateLabel;
    [Export] public Label WeatherLabel;
    
    [ExportSubgroup("Inventory Container")]
    [Export] public Control InventoryContainer;
    [Export] public Label NoItemsLabel;
    [Export] public VBoxContainer ItemsList;
    
    [ExportSubgroup("Fish Zone Container")]
    [Export] public Control FishZoneContainer;
    [Export] public Label NoFishLabel;
    [Export] public VBoxContainer FishList;
    
    [ExportSubgroup("Fishing Container")]
    [Export] public Control FishingContainer;
    [Export] public Label CurrentFishLabel;
    [Export] public Label FishTimerLabel;

    private readonly Dictionary<string, ItemCountRow> _inventoryRowMap = new();
    private readonly Dictionary<string, ItemCountRow> _fishZoneRowMap = new();
    private SceneTreeTimer _biteTimer = null;
    
    public override void _Ready()
    {
        GameManager.Instance.DebugUi = this;
        
        UpdateFishZoneContainer(null);
        HideFishingContainer();
    }

    public override void _Process(double delta)
    {
        UpdateTimeLabel();
        UpdateDateLabel();
        UpdateWeatherLabel();
        UpdateFishTimerLabel();
    }

    public void UpdateInventoryContainer()
    {
        if (GameManager.Instance.Inventory.Count == 0)
        {
            NoItemsLabel.Show();
            return;
        }
        
        NoItemsLabel.Hide();

        Dictionary<string, int> inventory = new();
        List<string> zeroItems = _inventoryRowMap.Keys.ToList();
        foreach (InventoryItem item in GameManager.Instance.Inventory)
        {
            zeroItems.Remove(item.Name);
            
            if (inventory.ContainsKey(item.Name)) inventory[item.Name] += 1;
            else inventory.Add(item.Name, 1);

            if (!_inventoryRowMap.ContainsKey(item.Name))
            {
                ItemCountRow row = Utility.LoadScene<ItemCountRow>("res://scenes/ui/item_count_row.tscn");
                
                ItemsList.AddChild(row);
                _inventoryRowMap.Add(item.Name, row);
            }

            _inventoryRowMap[item.Name].UpdateItemName(item.Name);
            _inventoryRowMap[item.Name].UpdateItemAmount(inventory[item.Name].ToString());
        }

        foreach (string itemName in zeroItems)
        {
            _inventoryRowMap[itemName].QueueFree();
            _inventoryRowMap.Remove(itemName);
        }
    }

    public void UpdateFishZoneContainer(FishZoneController fishZone)
    {
        if (fishZone == null)
        {
            FishZoneContainer.Hide();
            return;
        }
        
        FishZoneContainer.Show();

        WeightedPool<Fish> availableFish = fishZone.GetAvailableFishPool();
        
        if (availableFish.Elements.Count == 0)
        {
            NoFishLabel.Show();
            return;
        }
        
        NoFishLabel.Hide();

        List<string> zeroFish = _fishZoneRowMap.Keys.ToList();
        foreach (WeightedElement<Fish> fish in availableFish.Elements)
        {
            zeroFish.Remove(fish.Resource.Name);
            
            if (!_fishZoneRowMap.ContainsKey(fish.Resource.Name))
            {
                ItemCountRow row = Utility.LoadScene<ItemCountRow>("res://scenes/ui/item_count_row.tscn");
                
                FishList.AddChild(row);
                _fishZoneRowMap.Add(fish.Resource.Name, row);
            }

            float percentage = (fish.Weight / availableFish.TotalWeight) * 100f;
            _fishZoneRowMap[fish.Resource.Name].UpdateItemName(fish.Resource.Name);
            _fishZoneRowMap[fish.Resource.Name].UpdateItemAmount($"{percentage:0.00}%");
        }

        foreach (string fishName in zeroFish)
        {
            _fishZoneRowMap[fishName].QueueFree();
            _fishZoneRowMap.Remove(fishName);
        }
    }

    public void ShowFishingContainer(string fishName, SceneTreeTimer timer)
    {
        _biteTimer = timer;
        CurrentFishLabel.Text = fishName;
        FishingContainer.Show();
    }

    public void HideFishingContainer()
    {
        FishingContainer.Hide();
        _biteTimer = null;
    }

    private void UpdateTimeLabel()
    {
        float standardTime = GameManager.Instance.TimeOfDay * 24;
        
        int standardTimeHours = Mathf.FloorToInt(standardTime);
        int standardTimeMinutes = Mathf.RoundToInt((standardTime - standardTimeHours) * 60f); // Convert to minutes from float
        int nearestFiveMinInterval = Mathf.FloorToInt(standardTimeMinutes / 5f); // Every 5m
        double standardTimeMinutesRounded = nearestFiveMinInterval / 12d; // 12x 5m intervals in an hour
        
        TimeSpan time = TimeSpan.FromHours(standardTimeHours + standardTimeMinutesRounded);
        
        TimeLabel.Text = time.ToString("hh':'mm");
    }

    private void UpdateDateLabel()
    {
        DateLabel.Text = $"{GameManager.Instance.Season} {GameManager.Instance.DayOfSeason}, Year {GameManager.Instance.Year}";
    }

    private void UpdateWeatherLabel()
    {
        WeatherLabel.Text = GameManager.Instance.Weather.ToString();
    }

    private void UpdateFishTimerLabel()
    {
        if (_biteTimer == null) return;
        FishTimerLabel.Text = $"{_biteTimer.TimeLeft:0.00}s";
    }
}