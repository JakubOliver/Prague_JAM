using Godot;
using System;

public partial class MainScene : Node2D
{
	private AnimationPlayer AnimationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>("Transition/AnimationPlayer");
		//AnimationPlayer.get_parent().get_node("ColorRect")1.modulate = new Color(255, 255, 255, 1);
		AnimationPlayer.GetParent().GetNode<ColorRect>("ColorRect").Modulate = new Color(0, 0, 0, 0);
		AnimationPlayer.Play("fade_out");
		await ToSignal(AnimationPlayer, "animation_finished");
	}

	public override async void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("accept"))
		{
			AnimationPlayer.Play("fade_out");
			await ToSignal(AnimationPlayer, "animation_finished");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
