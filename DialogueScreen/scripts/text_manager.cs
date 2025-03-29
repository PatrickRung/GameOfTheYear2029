using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class text_manager : RichTextLabel
{	
	// Keeps track of when a chunk of text is finished printing
	// This is necessary to track typewriter effect timer
	[Signal]
	public delegate void ChunkFinishedEventHandler();
	
	// Keeps track of when a player clicks the screen
	[Signal]
	public delegate void ClickedEventHandler();
	
	[Export]
	public string FilePath = "";
	
	[Export]
	public float charactersPerSecond = 20.0f;
	
	private string _dialogueText;
	private int _currentCharacterIndex = 0;
	private Timer _timer;
	private bool _skipRequested = false;
	
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		if (!string.IsNullOrEmpty(FilePath))
		{
			// retrieves contents of file as a list of strings separated 
			// by delimiter '$END'
			List<string> data = ParseFile(FilePath);
			
			if (data == null || data.Count == 0) 
			{
				GD.PrintErr("Dialogue File is empty or failed to parse file");
				return;
			}
			
			// Prints each block of dialogue
			foreach (string block in data) 
			{
				_skipRequested = false;
				await PrintText(block); // wait for text to be fully printed
				await ToSignal(this, nameof(Clicked)); // Wait for click.
			}
		}
		else
		{
			GD.PrintErr("Dialogue File Path is Empty");
		}
	}

	private List<string> ParseFile(string filePath)
	{
		if (!FileAccess.FileExists(filePath)) 
		{
			GD.PrintErr($"Dialogue File not found: {filePath}");
			return null;
		}
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		if (file == null) 
		{
			GD.PrintErr($"Failed To Open Dialogue File: {filePath}");
			return null;
		}
		
		string fileContents = file.GetAsText();
		if (string.IsNullOrEmpty(fileContents))
			{
				GD.PrintErr($"Dialogue File is empty or could not be read: {filePath}");
				return new List<string>(); // Or throw an exception
		}
		
		string[] chunks = fileContents.Split("$END");
		
		return chunks.ToList();
	}
	
	private async System.Threading.Tasks.Task PrintText(string block)
	{
		_timer = new Timer();
		AddChild(_timer);
		_timer.WaitTime = 1.0f / charactersPerSecond;
		_timer.OneShot = false;
		_timer.Timeout += OnTimerTimeout;
		_timer.Start();

		Text = "";
		_currentCharacterIndex = 0;
		_dialogueText = block;

		await ToSignal(this, nameof(ChunkFinished)); // Wait for chunk to finish.
		_timer.QueueFree(); //Clean up the timer.
	}
	
	private void OnTimerTimeout()
	{
		if (_skipRequested)
		{
			Text = _dialogueText; // Display full text immediately.
			_timer.Stop();
			EmitSignal(nameof(ChunkFinished));
		} else if (_currentCharacterIndex <= _dialogueText.Length)
		{
			Text = _dialogueText.Substr(0, _currentCharacterIndex);
			_currentCharacterIndex++;
		}
		else
		{
			EmitSignal(nameof(ChunkFinished));
			_timer.Stop();
		}
	}
	
	// Handles mouse click input signal
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed)
		{
			if (mouseButtonEvent.ButtonIndex == MouseButton.Left)
			{
				_skipRequested = true;
				EmitSignal(nameof(Clicked)); // Emit click signal.
			}
		}
	}
	
}
