using System;
using FishingGame.Managers;
using Godot;

namespace FishingGame.Interface;

public partial class DebugUi : Control
{
    [Export] public Label TimeLabel;
    [Export] public Label DateLabel;

    public override void _Process(double delta)
    {
        UpdateTimeLabel();
        UpdateDateLabel();
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
        string season = GameManager.Instance.Season switch
        {
            0 => "Spring",
            1 => "Summer",
            2 => "Autumn",
            3 => "Winter",
            _ => throw new ArgumentOutOfRangeException()
        };

        DateLabel.Text = $"{season} {GameManager.Instance.DayOfSeason}, Year {GameManager.Instance.Year}";
    }
}