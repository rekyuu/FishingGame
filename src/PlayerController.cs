using FishingGame.Managers;
using FishingGame.Resources;
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
	[Export] public Node3D FishingRodModel;
	[Export] public Area3D FishingDetector;
	
	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	private float _verticalVelocity;
	
	private bool _isFishing = false;
	private bool _isHooking = false;
	
	private FishZoneController _currentFishingZone;
	private Fish _currentFish;
	private SceneTreeTimer _biteTimer;
	private SceneTreeTimer _hookTimer;

	public override void _Ready()
	{
		FishingRodModel.Visible = false;
		
		FishingDetector.BodyEntered += OnFishingDetectorBodyEntered;
		FishingDetector.BodyExited += OnFishingDetectorBodyExited;
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleControllerCamera();
		HandleMovement(delta);
		HandleFishing();
	}

	public override void _ExitTree()
	{
		FishingDetector.BodyEntered -= OnFishingDetectorBodyEntered;
		FishingDetector.BodyExited -= OnFishingDetectorBodyExited;
	}

	private void OnFishingDetectorBodyEntered(Node3D body)
	{
		if (body is not FishZoneController fishZone) return;
		
		_currentFishingZone = fishZone;
		GameManager.Instance.DebugUi.UpdateFishZoneContainer(fishZone);
		GameManager.Instance.InteractionsUi.ShowStartFishingLabel(true);
	}

	private void OnFishingDetectorBodyExited(Node3D body)
	{
		if (body is not FishZoneController) return;
		
		_currentFishingZone = null;
		GameManager.Instance.DebugUi.UpdateFishZoneContainer(null);
		GameManager.Instance.InteractionsUi.ShowStartFishingLabel(false);
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
		if (_isFishing) return;
		
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
		FishingDetector.Rotation = nextRotation;
	}

	private void HandleFishing()
	{
		if (_isFishing && _currentFishingZone == null) StopFishing();
		if (_currentFishingZone == null || !IsOnFloor()) return;
		
		if (Input.IsActionJustPressed("start_fishing") && !_isFishing) StartFishing();
		else if (Input.IsActionJustPressed("start_fishing") && _isFishing && !_isHooking) StartHooking();
		else if (Input.IsActionJustPressed("hook_fish") && _isFishing && _isHooking) HandleFishHook();
		else if (Input.IsActionJustPressed("stop_fishing") && _isFishing) StopFishing(); 
	}

	private void StartFishing()
	{
		GD.Print("You start fishing.");
		
		Velocity = Vector3.Zero;;
		
		_isFishing = true;
		FishingRodModel.Visible = true;
		GameManager.Instance.InteractionsUi.ShowStartFishingLabel(false);

		StartHooking();
	}

	private void StartHooking()
	{
		GD.Print("You cast your line.");
		
		_isHooking = true;
		_currentFish = _currentFishingZone.GetNextFish();
		float timeToBite = _currentFish.GetTimeToBite();
		
		_biteTimer = GetTree().CreateTimer(timeToBite);
		_biteTimer.Timeout += OnFishBite;
		
		GameManager.Instance.DebugUi.ShowFishingContainer(_currentFish.Name, _biteTimer);
	}

	private void OnFishBite()
	{
		if (_currentFish.Name == "NULL")
		{
			GD.Print("Nothing bites...");
			StopHooking();
			
			return;
		}
		
		GD.Print($"Bite!");
		
		Input.StartJoyVibration(0, 0.75f, 0.75f, 0.2f);
		
		_hookTimer = GetTree().CreateTimer(3);
		_hookTimer.Timeout += OnFishHookFailure;
	}

	private void HandleFishHook()
	{
		if (_hookTimer is { TimeLeft: > 0 })
		{
			int size = _currentFish.GetSize();
			GD.Print($"Caught a {_currentFish.Name} ({size}cm)");
			
			GameManager.Instance.AddToInventory(_currentFish);
		}
		else GD.Print($"Nothing bites...");
		
		StopHooking();
	}

	private void OnFishHookFailure()
	{
		GD.Print("The fish got away...");
		StopHooking();
	}

	private void StopHooking()
	{
		_isHooking = false;
		
		if (_biteTimer != null) _biteTimer.Timeout -= OnFishBite;
		if (_hookTimer != null) _hookTimer.Timeout -= OnFishHookFailure;
		
		_biteTimer = null;
		_hookTimer = null;
		GameManager.Instance.DebugUi.HideFishingContainer();
	}

	private void StopFishing()
	{
		GD.Print("You stop fishing.");
		
		StopHooking();
		
		_isFishing = false;
		FishingRodModel.Visible = false;
		GameManager.Instance.InteractionsUi.ShowStartFishingLabel(true);
	}
}
