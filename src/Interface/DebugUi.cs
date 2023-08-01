using System;
using Godot;

namespace FishingGame.Interface;

public partial class DebugUi : Control
{
    [Export] public Label TimeLabel;

    public override void _Process(double delta)
    {
        float standardTime = (float)Managers.GameManager.Instance.TimeOfDay * 24;
        TimeSpan time = TimeSpan.FromHours(standardTime);
        
        TimeLabel.Text = time.ToString("hh':'mm");
    }
}