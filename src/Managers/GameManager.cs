using Godot;

namespace FishingGame.Managers;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    public float TimeOfDay { get; private set; } = 0.7f;

    public float DayLengthSecs { get; private set; } = 20 * 60;

    private float _timeRate;

    public override void _Ready()
    {
        Instance = this;

        _timeRate = 1 / DayLengthSecs;
    }

    public override void _Process(double delta)
    {
        SetTimeOfDay(delta);
    }

    private void SetTimeOfDay(double delta)
    {
        float nextTimeOfDay = TimeOfDay + (_timeRate * (float)delta);
        if (nextTimeOfDay >= 1) nextTimeOfDay = 0;

        TimeOfDay = nextTimeOfDay;
    }
}