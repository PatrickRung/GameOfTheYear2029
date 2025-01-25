using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	public const int Speed = 2;
	public const float accel = 10;
	public const int MAXHEALTH = 100;
	public NavigationAgent3D nav_agent;
	public int health = MAXHEALTH;
	public ProgressBar progressBar;
	public Vector3 spawnArea; 

	public override void _Ready()
	{
		base._Ready();
		nav_agent = (NavigationAgent3D)GetNode("NavigationAgent3D");
		progressBar = (ProgressBar)GetNode("SubViewport/ProgressBar");
		progressBar.Value = health;
		spawnArea = GlobalPosition;
	}

	public override async void _PhysicsProcess(double delta)
	{
		//wait for next frame to do nav mesh code because navmesh does not generate on first frame
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		nav_agent.TargetPosition = ((CharacterBody3D)GetNode("/root/Game/Player")).Position;
		Vector3 direction = nav_agent.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();

		Velocity = Velocity.Lerp(direction * Speed, accel * (float)delta);

		MoveAndSlide();
	}
	
	private void _on_attack_range_body_entered(Node3D body) {
		GD.Print("test");
		if(body.Name.Equals("Player")) {
			((Player)body).dealDamae(50);
		}
	}

	public void dealDamae(int damage) {
		health -= damage;
		progressBar.Value = health;
	}
}
