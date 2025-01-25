using Godot;
using System;

public partial class Spawner : Node
{	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		Player player = ((Player)GetNode("/root/Game/Player"));
		Enemy currEnemy = ((Enemy)GetNode("/root/Game/Enemy"));
		CollisionShape3D collider = currEnemy.GetNode<CollisionShape3D>("CollisionShape3D");
		Area3D enemyAttackRange = currEnemy.GetNode<Area3D>("AttackRange");
		

		/* This only works for one enemy since we're referencing by name. For the future we either need 
		to seperate these calls to each possible enemy, or label each with a number (Enemy1, Enemy2, etc)
		to iterate thru them.*/
		if(player.respawned && !currEnemy.Visible) {
			GD.Print("Enemy respawned");
			currEnemy.Visible = true;
			currEnemy.SetProcess(true);
			currEnemy.SetPhysicsProcess(true);
			collider.SetDisabled(false);
			enemyAttackRange.SetMonitoring(true);
			currEnemy.GlobalPosition = currEnemy.spawnArea;
			currEnemy.health = 100;
			player.respawned = false;
		}


		if(currEnemy.health <= 0 && currEnemy.Visible) {
			GD.Print("Enemy killed");
			currEnemy.Visible = false;
			currEnemy.SetProcess(false);
			currEnemy.SetPhysicsProcess(false);
			collider.SetDisabled(true);
			enemyAttackRange.SetMonitoring(false);
		}
	}
}
