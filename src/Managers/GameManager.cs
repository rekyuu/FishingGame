using Godot;

namespace FishingGame.Managers;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    
    public double TimeOfDay { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        SetTimeOfDay(delta);
    }

    private void SetTimeOfDay(double delta)
    {
        double nextTimeOfDay = TimeOfDay + delta;
        if (nextTimeOfDay >= 1800) nextTimeOfDay = 0;

        TimeOfDay = nextTimeOfDay;
    }
}