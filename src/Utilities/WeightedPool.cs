using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FishingGame.Utilities;

public class WeightedPool<T> where T : Resource
{
    [Export] public List<WeightedElement<T>> Elements { get; set; } = new();

    public float TotalWeight { get; private set; } = 0;

    public WeightedPool()
    {
        UpdateWeights();
    }

    public void UpdateWeights()
    {
        TotalWeight = 0;
        Elements = Elements.OrderBy(x => x.Weight).ToList();
        
        foreach (WeightedElement<T> element in Elements)
        {
            TotalWeight += element.Weight;
            element.AccumulatedWeight = TotalWeight;
        }
    }

    public T GetRandomElement()
    {
        Random r = new();
        float result = r.NextSingle() * TotalWeight;

        return Elements.First(x => x.AccumulatedWeight >= result).Resource;
    }
}