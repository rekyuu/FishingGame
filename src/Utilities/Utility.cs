using System;
using System.Text.Json;
using Godot;

namespace FishingGame.Utilities;

public static class Utility
{
    public static T LoadScene<T>(string path) where T : Node
    {
        PackedScene scene = GD.Load<PackedScene>(path);
        return LoadScene<T>(scene);
    }
    
    public static T LoadScene<T>(PackedScene scene) where T : Node
    {
        return scene.Instantiate<T>();
    }
    
    public static float GetNextFloat(float min, float max)
    {
        Random r = new();
        return (r.NextSingle() * (max - min)) + min;
    }

    public static T LoadFromJson<T>(string resourcePath)
    {
        using FileAccess file = FileAccess.Open(resourcePath, FileAccess.ModeFlags.Read);
        string data = file.GetAsText();
        
        return JsonSerializer.Deserialize<T>(data);
    }
}