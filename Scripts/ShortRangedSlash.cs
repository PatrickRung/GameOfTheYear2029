using Godot;
using System;

public partial class ShortRangedSlash : Area3D
{
	private int damage = 50;
	private Timer timer;
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.Start();
		GD.Print("spawn");
	}


	public void _on_timer_timeout() {
		QueueFree();
		GD.Print("test");
	}

	public void _on_body_entered(Node3D hit) {
		if(hit.Name.Equals("Enemy")) {
			((Enemy)hit).dealDamae(damage);
		}
	}
}
