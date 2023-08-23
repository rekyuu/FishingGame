using Godot;

namespace FishingGame.Interface;

public partial class ItemCountRow : Control
{
    [Export] public Label ItemNameLabel;
    [Export] public Label ItemCountLabel;

    public void UpdateItemName(string name)
    {
        ItemNameLabel.Text = name;
    }

    public void UpdateItemAmount(string count)
    {
        ItemCountLabel.Text = count;
    }
}