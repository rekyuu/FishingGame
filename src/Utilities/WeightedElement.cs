using Godot;

namespace FishingGame.Utilities;

public class WeightedElement<T> where T : Resource
{
    [Export] public float Weight { get; set; }
    
    [Export] public T Resource { get; set; }

    public float AccumulatedWeight { get; set; } = 0;
}