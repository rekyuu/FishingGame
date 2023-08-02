using FishingGame.Managers;
using Godot;

namespace FishingGame.Interface;

public partial class InteractionsUi : Control
{
    [Export] public Control StartFishingLabel;

    public override void _Ready()
    {
        GameManager.Instance.Interactions = this;
        
        StartFishingLabel.Visible = false;
    }

    public void ShowStartFishingLabel(bool show)
    {
        StartFishingLabel.Visible = show;
    }
}