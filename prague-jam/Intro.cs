
using Godot;
using System;
using System.Threading.Tasks;

public partial class Intro : Node2D
{
	private AnimationPlayer AnimationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>("Transition/AnimationPlayer");
	}

	public override async void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("accept"))
		{
			AnimationPlayer.Play("fade_in");
			await ToSignal(AnimationPlayer, "animation_finished");

			GetTree().ChangeSceneToFile("res://intro_cut_scene.tscn");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
