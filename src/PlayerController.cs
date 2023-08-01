using Godot;

namespace FishingGame;

public partial class PlayerController : CharacterBody3D
{
    [Export] public float Speed = 10.0f;
    [Export] public float Acceleration = 15.0f;
    [Export] public float AirAcceleration = 5.0f;
    [Export] public float MaxTerminalVelocity = 54.0f;
    [Export] public float JumpVelocity = 15.0f;

    [Export] public float ControllerCameraSensitivity = 3.0f;
    [Export] public bool InvertXAxis = false;
    [Export] public bool InvertYAxis = false;
    [Export(PropertyHint.Range, "-90, 0")] public float MinPitch = -30.0f;
    [Export(PropertyHint.Range, "0, 90")] public float MaxPitch = 90.0f;
    
    [Export] public Node3D CameraPivot;
    [Export] public Node3D PlayerModel;
    
    private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    
    private float _verticalVelocity;

    public override void _PhysicsProcess(double delta)
    {
        HandleControllerCamera();
        HandleMovement(delta);
    }

    private void HandleControllerCamera()
    {
        float invertX = InvertXAxis ? -1 : 1;
        float invertY = InvertYAxis ? -1 : 1;
        
        Vector2 cameraAxis = Input.GetVector(
            "camera_left",
            "camera_right",
            "camera_up",
            "camera_down");

        AdjustCamera(
            cameraAxis.X * invertX * ControllerCameraSensitivity, 
            cameraAxis.Y * invertY * ControllerCameraSensitivity);
    }

    private void AdjustCamera(float xAxis, float yAxis)
    {
        Vector3 nextCameraRotationDegrees = CameraPivot.RotationDegrees;

        nextCameraRotationDegrees.X -= yAxis;
        nextCameraRotationDegrees.Y -= xAxis;
        nextCameraRotationDegrees.X = Mathf.Clamp(nextCameraRotationDegrees.X, -MaxPitch, -MinPitch);

        CameraPivot.RotationDegrees = nextCameraRotationDegrees;
    }

    private void HandleMovement(double delta)
    {
        Basis cameraBasis = CameraPivot.Transform.Basis;
        Vector3 cameraX = cameraBasis.X;
        Vector3 cameraZ = cameraBasis.Z;
        cameraX.Y = 0;
        cameraZ.Y = 0;
        cameraX = cameraX.Normalized();
        cameraZ = cameraZ.Normalized();
        float invert = Mathf.Abs(CameraPivot.RotationDegrees.X) == 90 ? -1 : 1;

        Vector2 inputDir = Input.GetVector(
            "move_left",
            "move_right",
            "move_up",
            "move_down");
        Vector3 direction = (inputDir.Y * cameraZ * invert) + (inputDir.X * cameraX);

        float nextAcceleration = IsOnFloor() ? Acceleration : AirAcceleration;
        Vector3 nextVelocity = Velocity;
        
        nextVelocity = nextVelocity.Lerp(direction * Speed, nextAcceleration * (float)delta);
        _verticalVelocity = IsOnFloor() ? -0.01f : Mathf.Clamp(_verticalVelocity - _gravity, -MaxTerminalVelocity, MaxTerminalVelocity);

        if (Input.IsActionJustPressed("jump") && IsOnFloor()) _verticalVelocity = JumpVelocity;

        nextVelocity.Y = _verticalVelocity;

        Velocity = nextVelocity;
        MoveAndSlide();

        if (direction == Vector3.Zero) return;
        
        float angle = Mathf.Atan2(-Velocity.X, -Velocity.Z);
        Vector3 nextRotation = PlayerModel.Rotation;
        nextRotation.Y = angle;
            
        PlayerModel.Rotation = nextRotation;
    }
}