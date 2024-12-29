using Godot;
using System;

public partial class HealthPot : Area3D
{
	public override void _Ready()
	{

	}
	
	public void _on_body_entered(Node3D body) {
		if(body.Name.Equals("Player")) {
			Player playerScript = body as Player;
			playerScript.updateItemBar();
			QueueFree(); 
			
		}
	}
}