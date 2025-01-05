using Godot;
using System;

public partial class HealthPot : Item
{
	public override void _Ready()
	{
        this.currItemAttributes = new itemAttributes("HealthPot", false, true);
        this.currItemAttributes.healValue = 20;
	}
}
