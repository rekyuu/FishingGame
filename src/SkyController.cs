using FishingGame.Managers;
using Godot;

namespace FishingGame;

public partial class SkyController : Node
{
    [Export(PropertyHint.Range, "0,1,0.01")]
    public float TimeOfDay
    {
        get => _timeOfDay;
        set
        {
            SetSkyTime(_timeOfDay);
            _timeOfDay = value;
        }
    }

    [Export] public WorldEnvironment Environment;
    [Export] public Gradient SkyTopColor;
    [Export] public Gradient SkyHorizonColor;
    
    [Export] public Node3D CelestialBodies;
    [Export] public DirectionalLight3D Sun;
    [Export] public DirectionalLight3D Moon;

    [Export] public Gradient SunColor;
    [Export] public Curve SunIntensity;
    [Export] public Gradient MoonColor;
    [Export] public Curve MoonIntensity;

    private float _timeOfDay = 0.33f;

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint()) return;
        
        TimeOfDay = GameManager.Instance.TimeOfDay;
        SetSkyTime(TimeOfDay);
    }

    public void SetSkyTime(float timeOfDay)
    {
        Vector3 celestialBodiesRotation = CelestialBodies.RotationDegrees;
        celestialBodiesRotation.Z = (timeOfDay * 360) - 90;
        
        // Environment.Environment.Sky.SkyMaterial.Set("sky_top_color", SkyTopColor.Sample(timeOfDay));
        // Environment.Environment.Sky.SkyMaterial.Set("sky_horizon_color", SkyHorizonColor.Sample(timeOfDay));
        // Environment.Environment.Sky.SkyMaterial.Set("ground_horizon_color", SkyHorizonColor.Sample(timeOfDay));
        // Environment.Environment.Sky.SkyMaterial.Set("ground_bottom_color", SkyHorizonColor.Sample(timeOfDay));
        
        CelestialBodies.RotationDegrees = celestialBodiesRotation;
        
        Sun.LightColor = SunColor.Sample(timeOfDay);
        Sun.LightEnergy = SunIntensity.Sample(timeOfDay);
        
        Moon.LightColor = MoonColor.Sample(timeOfDay);
        Moon.LightEnergy = MoonIntensity.Sample(timeOfDay);
    }
}