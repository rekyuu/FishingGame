using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FishingGame.Utilities;

public class WeightedPool<T> where T : Resource
{
    [Export] public List<WeightedElement<T>> Elements { get; set; } = new();

    private float _totalWeight = 0;

    public WeightedPool()
    {
        UpdateWeights();
    }

    public void UpdateWeights()
    {
        _totalWeight = 0;
        Elements = Elements.OrderBy(x => x.Weight).ToList();
        
        foreach (WeightedElement<T> element in Elements)
        {
            _totalWeight += element.Weight;
            element.AccumulatedWeight = _totalWeight;
        }
    }

    public T GetRandomElement()
    {
        Random r = new();
        float result = r.NextSingle() * _totalWeight;

        return Elements.First(x => x.AccumulatedWeight >= result).Resource;
    }
}