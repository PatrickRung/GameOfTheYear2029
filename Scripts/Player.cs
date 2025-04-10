using Godot;
using System;

public partial class Player : CharacterBody3D
{
	//Character movement
	public const float Speed = 5.0f;
	public const float reachDistance = 1000;
	public const float JumpVelocity = 4.5f;

	//Character progressBars
	public int health = 100;
	private int stamina = 100;


	// Stamina regeneration
	private double regenSpeed = 1.5;
	private double regenTimer = 0;

	//smaller the number of the attack speed the faster
	private double attackSpeed = 1.5;
	
	//Player Viewing
	public ProgressBar healthBar;
	public ProgressBar staminaBar;
	public Camera3D Camera;

	//melee data
	private Node melee2D;
	private bool attacked;
	private double meleeTimer;
	private Vector3 nonZeroDir;
	
	//we will have documentation in a file to correlate items to numbers
	public itemAttributes[] inventory = new itemAttributes[4];
	private Sprite2D Item1, Item2, Item3, Item4;
	private Sprite2D[] selectedBar;	
	int spotHeld;
	//to be instantiated nodes, make sure to preload
	PackedScene shortRangedReach;
	


	public override void _Ready() {
		// Accessess all of the elements in the HUD
		Item1 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item1");
		Item2 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item2");
		Item3 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item3");
		Item4 = GetNode<Sprite2D>("/root/Game/CanvasLayer/Item4");
		healthBar = GetNode<ProgressBar>("/root/Game/CanvasLayer/ControlHUD/ProgressBars/healthBar");
		staminaBar = GetNode<ProgressBar>("/root/Game/CanvasLayer/ControlHUD/ProgressBars/staminaBar");

		int counter = 0;
		selectedBar = new Sprite2D[4];
		foreach (Node currNode in GetNode("/root/Game/CanvasLayer/SelectedHolder").GetChildren()) {
			selectedBar[counter] = ((Sprite2D)currNode);
			counter++;
		}
		Camera = GetNode<Camera3D>("/root/Game/Player/Camera3D");
		healthBar.Value = health;
		staminaBar.Value = stamina;
		shortRangedReach = GD.Load<PackedScene>("res://short_ranged_slash.tscn");
		for(int i = 0; i < inventory.Length; i++) {
			inventory[i] = new itemAttributes("NULL", false, false);
		}

		//updates item bar to have the right items held and hand selected
		updateItemBar();
		selectedItemBarHeld();

		//must be assigned
		attacked = false;
		meleeTimer = 0;
		spotHeld = 0;
		nonZeroDir = new Vector3(0, 0, -1);
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
			nonZeroDir = direction;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();


		if(Input.IsActionJustReleased("left_click")) {
			if(!attacked && inventory[spotHeld].isWeapon && this.stamina >= 30) {
				// Attacking
				// if the player cooldown is off and a weapon is equipped, allow attacking
				attacked = true;
				melee2D = (Area3D)shortRangedReach.Instantiate();
				AddChild(melee2D);
				((Area3D)melee2D).Position = nonZeroDir;
				
				// if the player has enough stamina left he will execute the 
				// (jab) attack, and reduce the stamina 
				this.stamina -= 30;
				staminaBar.Value = this.stamina;
				GD.Print("lost stamina:" + this.stamina);
			}
			else if(Input.IsActionJustReleased("left_click") 
							&& inventory[spotHeld].isConsumable) {
				//Healing
				dealDamage(-inventory[spotHeld].healValue);
				inventory[spotHeld] = new itemAttributes("NULL", false, false); 
				updateItemBar();
			}

		}

		// If our player is attacked, disable our player's ability to attack momentarily
		if(attacked) {
			meleeTimer += delta;
			if(meleeTimer >= attackSpeed) {
				attacked = false;
				meleeTimer = 0;
			}
		}

		// As our player decides to attack, we will slowly regenerate the stamina
		// only if the stamina is below the max threshold
		if(this.stamina < 100) {
			regenTimer += delta;
			if(regenTimer >= regenSpeed) {
				this.stamina += 5;
				regenTimer = 0;
			}
			staminaBar.Value = this.stamina;
		}

		// Inventory
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

	// @function: Updates the player's inventory showing 
	// images of what the player holds in the HUD
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

	// @parameter: Accepts an int value that represents the damage that
	// our player has taken
	// @function: Updates the player's health accordingly to the inputted
	// damage parameter. In the case our player's health runs below or equal 
	// to zero, we declare our player as dead, and reload the scene. 
	public void dealDamage(int damage) {
		health -= damage;
		healthBar.Value = health;
		if(health <= 0) {
			GetTree().ReloadCurrentScene();
		}
	}
}
