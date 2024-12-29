using Godot;
using System;

public partial class Player : CharacterBody3D
{
	//assignables
	public const float Speed = 5.0f;
	public const float reachDistance = 1000;
	public const float JumpVelocity = 4.5f;
	public int health = 100;
	//Node declaration and access
	public ProgressBar healthBar;
	public Camera3D Camera;
	
	//we will have documentation in a file to correlate items to numbers
	public int[] inventory = new int[4];
	private Sprite2D Item1, Item2, Item3, Item4;

	public override void _Ready() {
		Item1 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item1");
		Item2 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item2");
		Item3 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item3");
		Item4 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item4");
		healthBar = GetNode<ProgressBar>("/root/Game/CanvasLayer/healthBar");
		Camera = GetNode<Camera3D>("/root/Game/Player/Camera3D");
		healthBar.Value = health;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		//REMOVED JUMP FROM GAME. Idea is that since this is meant to imatate a 2d sidescroller but in a 3d engine 
		//jumping doesn't make much sense. Also it's hard to implement juimping attack hitboxes and I am lazy
		//if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		//{
			//velocity.Y = JumpVelocity;
		//}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		//Changed to WASD
		Vector2 inputDir = Input.GetVector("key_left", "key_right", "key_up", "key_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
		

	}

	public void updateItemBar() {
		Item1.Texture = (Texture2D)GD.Load("res://itemTextures/Untitled.png");
	}
}