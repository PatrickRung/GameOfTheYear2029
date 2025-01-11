using Godot;
using System;

public partial class Knife : Item
{
	public override void _Ready()
	{
        this.currItemAttributes = new itemAttributes("Knife", true, false);
	}
}
