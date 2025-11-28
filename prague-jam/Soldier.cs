using Godot;
using System;

public partial class Soldier : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	private AnimatedSprite2D _animatedSprite2D;
	
	private void OnBodyEntered(Node2D body) {
		
		GD.Print("třeba ahoj");
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite2D.Play("idle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
