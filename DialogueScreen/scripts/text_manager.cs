using Godot;
using System;

public partial class text_manager : RichTextLabel
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		string path = "res://text/test.json";
		
		string data = LoadFileAsText(path);

		if (!string.IsNullOrEmpty(data))
		{
			 Text = data;
		}
		else
		{
			GD.PrintErr("Failed to load or parse the file at: " + path);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private string LoadFileAsText(string filePath)
	{
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		return file.GetAsText();
	}
}
