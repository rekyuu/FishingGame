using System.Collections.Generic;
using FishingGame.Interface;
using FishingGame.Resources;
using Godot;

namespace FishingGame.Managers;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    
    public DebugUi DebugUi { get; set; }
    
    public InteractionsUi InteractionsUi { get; set; }

    public List<InventoryItem> Inventory { get; private set; } = new();

    public int InventoryLimit { get; private set; } = 24;

    public float TimeOfDay { get; private set; } = 18f / 24f;

    public Weather Weather { get; private set; } = Weather.Clear;

    public Season Season { get; private set; } = Season.Spring;
    
    public int DayOfSeason { get; private set; } = 1;

    public int Year { get; private set; } = 1;

    private float _dayLengthSecs = 20 * 60;
    private float _timeRate;

    public override void _Ready()
    {
        Instance = this;
        
        _timeRate = 1 / _dayLengthSecs;
    }

    public override void _Process(double delta)
    {
        SetTimeOfDay(delta);
    }

    public void AddToInventory(InventoryItem item)
    {
        Inventory.Add(item);
        DebugUi.UpdateInventoryContainer();
    }

    private void SetTimeOfDay(double delta)
    {
        TimeOfDay += _timeRate * (float)delta;
        if (TimeOfDay < 1) return;
        
        TimeOfDay = 0;
        DayOfSeason++;
        if (DayOfSeason <= 28) return;
        
        DayOfSeason = 1;
        Season++;
        if ((int)Season <= 3) return;
        
        Season = 0;
        Year++;
    }
}