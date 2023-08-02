using System.Text.Json.Serialization;

namespace FishingGame.Data;

public class FishZoneData
{
    [JsonPropertyName("fish")]
    public string FishName { get; set; }
    
    [JsonPropertyName("weight")]
    public float FishWeight { get; set; }
}