using Godot;
using System;

public partial class Checkpoint : Area3D
{
	[Signal] 
	public delegate void CheckpointActivatedEventHandler(Vector3 respawnPosition);

	private Marker3D respawnPoint;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		respawnPoint = GetNode<Marker3D>("RespawnPoint");
		if(respawnPoint == null) {
			GD.PrintErr("Respawn point (Marker3D) not found!");
		} 

		
		this.Connect("body_entered", Callable.From((Node3D body) => { OnBodyEntered(body); }));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void OnBodyEntered(Node3D body) {
		if(body.Name.Equals("Player")) {
			if(respawnPoint != null) {
				EmitSignal("CheckpointActivatedEventHandler", respawnPoint.GlobalPosition);
				GD.Print("Checkpoint Activated");
				((Player)body).onCheckpointActivated(respawnPoint.GlobalPosition);
			}
		}
	}

	
}
