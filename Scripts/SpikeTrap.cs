using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class SpikeTrap : MeshInstance3D
{
	// Called when the node enters the scene tree for the first time.
	//sound effects are from this website
	//https://freesound.org/
	private protected AnimationPlayer spikeAnimPlayer;
	private protected AudioStreamPlayer3D SpikeAudio;

	private Boolean triggered, spikeActivated;
	private double triggeredTime;
	private static double spikeDelaytime = 2;
	private static double spikeCoolDownTime = 7;
	private Node3D inHitBox;
	
	public override void _Ready()
	{
		spikeAnimPlayer = (AnimationPlayer)this.FindChild("AnimationPlayer");	
		SpikeAudio = (AudioStreamPlayer3D)this.FindChild("SpikeSoundPlayer");	
		triggered = false;
		spikeActivated = false;
		triggeredTime = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(triggered) {
			triggeredTime += delta;
		}
		if(triggeredTime > spikeDelaytime && !spikeActivated) {
			spikeActivated = true;
			spikeAnimPlayer.Play("spikeEnable");
			SpikeAudio.Play();
			if(inHitBox != null && inHitBox.Name.Equals("Player")) {
				Player playerObject = (Player)inHitBox;
				playerObject.dealDamae(20);
				
			}
		}
		if (triggeredTime > spikeCoolDownTime) {
			triggeredTime = 0;
			spikeActivated = false;
			triggered = false;
		}
	}

	public void _on_area_3d_body_entered(Node3D hit) {
		if(!triggered) {
			GD.Print("entered");
			triggered = true;
		}
		inHitBox = hit;

	}

	public void _on_area_3d_body_exited(Node3D hit) {
		inHitBox = null;
	}
}
