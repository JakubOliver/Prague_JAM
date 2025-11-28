using Godot;
using System;

public partial class Soldier : Area2D, Person
{
	[Signal]
	public delegate void HitEventHandler();
	
	[Export]	
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;

	public int HP { get; set; } = 100;
	public int Damage { get; set; } = 10;

	public PersonClothing head { get; set; } = PersonClothing.Player;
	public PersonClothing body { get; set; } = PersonClothing.Player;
	public PersonClothing weapon { get; set; } = PersonClothing.Player;

	public void Start(Vector2 position)
	{
		Position = position;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	private void OnBodyEntered(Node2D node){
		GD.Print("ahoj");
	}
}
