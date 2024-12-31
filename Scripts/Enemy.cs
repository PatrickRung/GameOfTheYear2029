using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	public const int Speed = 2;
	public const float accel = 10;
	public NavigationAgent3D nav_agent;
	public int health = 100;
	public ProgressBar progressBar;
	public override void _Ready()
	{
		base._Ready();
		nav_agent = (NavigationAgent3D)GetNode("NavigationAgent3D");
		progressBar = (ProgressBar)GetNode("SubViewport/ProgressBar");
		progressBar.Value = health;
	}

	public override void _PhysicsProcess(double delta)
	{
		nav_agent.TargetPosition = ((CharacterBody3D)GetNode("/root/Game/Player")).Position;
		Vector3 direction = nav_agent.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();

		Velocity = Velocity.Lerp(direction * Speed, accel * (float)delta);


		MoveAndSlide();
	}

	public void dealDamae(int damage) {
		health -= damage;
		progressBar.Value = health;
		if(health <= 0) {
			QueueFree();
		}
	}
}
