using System;
using System.Text.Json;
using Godot;

namespace FishingGame.Utilities;

public static class Utility
{
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