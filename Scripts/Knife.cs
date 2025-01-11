using Godot;
using System;

public partial class Knife : Area3D
{
	const int damage = 100;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_body_entered(Node3D body) {
		if(body.Name.Equals("Enemy")) {
			
		}
	}
}
