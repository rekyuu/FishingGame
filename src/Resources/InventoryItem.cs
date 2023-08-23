using Godot;

namespace FishingGame.Resources;

public partial class InventoryItem : Resource
{
    [Export]
    public string Name { get; set; }
}