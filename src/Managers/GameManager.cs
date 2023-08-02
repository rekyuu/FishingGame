using FishingGame.Interface;
using Godot;

namespace FishingGame.Managers;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    
    public InteractionsUi Interactions { get; set; }

    public float TimeOfDay { get; private set; } = 8f / 24f;

    public int Season { get; private set; } = 0;
    
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

    private void SetTimeOfDay(double delta)
    {
        TimeOfDay += _timeRate * (float)delta;
        if (TimeOfDay < 1) return;
        
        TimeOfDay = 0;
        DayOfSeason++;
        if (DayOfSeason <= 28) return;
        
        DayOfSeason = 1;
        Season++;
        if (Season <= 3) return;
        
        Season = 0;
        Year++;
    }
}