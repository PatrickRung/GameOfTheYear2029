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
	public itemAttributes[] inventory = new itemAttributes[4];
	private Sprite2D Item1, Item2, Item3, Item4;
	private Sprite2D[] selectedBar;	
	int spotHeld;
	//to be instantiated nodes, make sure to preload
	PackedScene shortRangedReach;
	


	public override void _Ready() {
		Item1 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item1");
		Item2 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item2");
		Item3 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item3");
		Item4 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item4");
		healthBar = GetNode<ProgressBar>("/root/Game/CanvasLayer/healthBar");
		int counter = 0;
		selectedBar = new Sprite2D[4];
		foreach (Node currNode in GetNode("/root/Game/CanvasLayer/SelectedHolder").GetChildren()) {
			selectedBar[counter] = ((Sprite2D)currNode);
			counter++;
		}
		Camera = GetNode<Camera3D>("/root/Game/Player/Camera3D");
		healthBar.Value = health;
		shortRangedReach = GD.Load<PackedScene>("res://short_ranged_slash.tscn");
		for(int i = 0; i < inventory.Length; i++) {
			inventory[i] = new itemAttributes("NULL", false, false);
		}

		//updates item bar to have the right items held and hand selected
		updateItemBar();
		selectedItemBarHeld();

		//must be asigned
		attacked = false;
		meleeTimer = 0;
		spotHeld = 0;
	}

	public override void _PhysicsProcess(double delta)
	{
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


		if(Input.IsActionJustReleased("left_click")) {
			if(!attacked && inventory[spotHeld].isWeapon) {
				//Attacking
				//if the player cooldown is off and the current item equiped is a weapon allow attacking
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
			else if(Input.IsActionJustReleased("left_click") && inventory[spotHeld].isConsumable) {
				//Healing
				dealDamae(-inventory[spotHeld].healValue);
				inventory[spotHeld] = new itemAttributes("NULL", false, false); 
				updateItemBar();
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
			spotHeld = 0;
			selectedItemBarHeld();
		} else if(Input.IsActionJustReleased("slot_two")){
			spotHeld = 1;
			selectedItemBarHeld();
		} else if(Input.IsActionJustReleased("slot_three")){
			spotHeld = 2;
			selectedItemBarHeld();
		} else if(Input.IsActionJustReleased("slot_four")){
			spotHeld = 3;
			selectedItemBarHeld();
		}
		

	}
	public void selectedItemBarHeld() {
		for(int i = 0; i < 4; i++) {
			if(i != spotHeld) {
				selectedBar[i].Visible = false;
			}
			else {
				selectedBar[i].Visible = true;
			}
		}
	}

	public void updateItemBar() {
		String location = "res://itemTextures/" + inventory[0].name + ".png";
		Item1.Texture = (Texture2D)GD.Load(location);
		location = "res://itemTextures/" + inventory[1].name + ".png";
		Item2.Texture = (Texture2D)GD.Load(location);
		location = "res://itemTextures/" + inventory[2].name + ".png";
		Item3.Texture = (Texture2D)GD.Load(location);
		location = "res://itemTextures/" + inventory[3].name + ".png";
		Item4.Texture = (Texture2D)GD.Load(location);
	}


	//returns evicted item as string
	public String addItem(itemAttributes itemName) {
		String evicted = "Nothing";
		for(int i = 0; i < 4; i++) {
			if(inventory[i].name == "NULL") {
				inventory[i] = itemName;
				updateItemBar();
				return evicted;
			}
		}
		inventory[0] = itemName;
		updateItemBar();
		return inventory[0].name;
	}

	public void dealDamae(int damage) {
		health -= damage;
		healthBar.Value = health;
		if(health <= 0) {
			GetTree().ReloadCurrentScene();
		}
	}
}
