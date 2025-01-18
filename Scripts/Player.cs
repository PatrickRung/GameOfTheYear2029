using Godot;
using System;

public partial class Player : CharacterBody3D
{
	//assignables
	public const float Speed = 5.0f;
	public const float reachDistance = 1000;
	public const float JumpVelocity = 4.5f;
	public int health = 100;
	//smaller the number of the attack speed the faster
	private double attackSpeed = 1.5;
	//Node declaration and access
	public ProgressBar healthBar;
	public Camera3D Camera;

	//melee data
	private Node melee2D;
	private bool attacked;
	private double meleeTimer;
	
	//we will have documentation in a file to correlate items to numbers
	public String[] inventory = new String[4];
	private Sprite2D Item1, Item2, Item3, Item4;

	//to be instantiated nodes, make sure to preload
	PackedScene shortRangedReach;
<<<<<<< Updated upstream


=======
	
	// stores the last checkpoint that our player has visited
	private Vector3 lastCheckpoint;
	private bool isDead = false; // tells us whether our character has died recently
>>>>>>> Stashed changes

	public override void _Ready() {
		Item1 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item1");
		Item2 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item2");
		Item3 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item3");
		Item4 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item4");
		healthBar = GetNode<ProgressBar>("/root/Game/CanvasLayer/healthBar");
		Camera = GetNode<Camera3D>("/root/Game/Player/Camera3D");
		healthBar.Value = health;
		shortRangedReach = GD.Load<PackedScene>("res://short_ranged_slash.tscn");
		for(int i = 0; i < inventory.Length; i++) {
			inventory[i] = null;
		}
		updateItemBar();
		attacked = false;
		meleeTimer = 0;
<<<<<<< Updated upstream
=======
		spotHeld = 0;

		//holds the initial checkpoint that our player starts at
		lastCheckpoint = GlobalPosition;
>>>>>>> Stashed changes
	}

	public override void _PhysicsProcess(double delta)
	{
		if(isDead) {
			GD.Print("Player Died");
			Respawn();
		}
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

		//Attacking
		if(Input.IsActionJustReleased("left_click") && !attacked) {
			attacked = true;
			melee2D = (Area3D)shortRangedReach.Instantiate();
			AddChild(melee2D);
			if(direction.Equals(new Vector3(0,0,0))) {
				((Area3D)melee2D).Position = new Vector3(0,0,-1);
			}
			else {
				((Area3D)melee2D).Position = direction;
			}
		}

		if(attacked) {
			meleeTimer += delta;
			if(meleeTimer >= attackSpeed) {
				attacked = false;
				meleeTimer = 0;
			}
		}

		//Inventory
		if(Input.IsActionJustReleased("slot_one")) {

		} else if(Input.IsActionJustReleased("slot_two")){

		} else if(Input.IsActionJustReleased("slot_three")){

		} else if(Input.IsActionJustReleased("slot_four")){

		}


	}

	public void updateItemBar() {
		String location = "res://itemTextures/" + inventory[0] + ".png";
		Item1.Texture = (Texture2D)GD.Load(location);
		location = "res://itemTextures/" + inventory[1] + ".png";
		Item2.Texture = (Texture2D)GD.Load(location);
		location = "res://itemTextures/" + inventory[2] + ".png";
		Item3.Texture = (Texture2D)GD.Load(location);
		location = "res://itemTextures/" + inventory[3] + ".png";
		GD.Print(location);
		Item4.Texture = (Texture2D)GD.Load(location);
	}


	//returns evicted item as string
	public String addItem(String itemName) {
		String evicted = "Nothing";
		for(int i = 0; i < 4; i++) {
			if(inventory[i] == null ) {
				inventory[i] = itemName;
				return evicted;
			}
		}
		inventory[0] = itemName;
		return inventory[0];
	}


	// Might be better to chagne this to damageDealt, so that we understand it's damage being dealt to our player
	/*
		This method removes a value of health points from our health bar depending on the integer value inputted. If the damage dealt
		results in our characters health dropping below or equal to 0 we kill the character, respawn all enemies, and respawn player
		at last known checkpoint with their inventory still intact. 
	*/
	public void dealDamae(int damage) {
		health -= damage;
		healthBar.Value = health;
		if(health <= 0) {
			isDead = true;
			// GetTree().ReloadCurrentScene();
		}
	}

	private void Respawn() {
		GlobalPosition = lastCheckpoint;
		Velocity = Vector3.Zero;
		isDead = false;
		GD.Print("Player's position changed to last known checkpoint");
	}

	public void onCheckpointActivated(Vector3 checkpointPosition) {
		lastCheckpoint = checkpointPosition;
		GD.Print("Players last known checkpoint changed");
	}
}
