using Godot;
using System;

public partial class SpikeTrap : MeshInstance3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void _on_area_3d_body_entered(Node3D hit) {
		GD.Print("entered spike range");
		if(hit.Name.Equals("Player")) {
			Player playerObject = (Player)hit;
			playerObject.dealDamae(20);
		}
	}
}
