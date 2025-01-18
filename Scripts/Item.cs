using Godot;
using System;

public partial class Item : Area3D
{
	public itemAttributes currItemAttributes;
	
	public void _on_body_entered(Node3D body) {
		if(body.Name.Equals("Player")) {
			Player playerScript = body as Player;
			playerScript.addItem(currItemAttributes);
			QueueFree(); 
			
		}
	}
}
