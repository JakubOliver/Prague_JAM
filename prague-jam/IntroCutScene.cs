using Godot;
using System;

public partial class IntroCutScene : Node2D
{
	public AnimatedSprite2D AnimatedSpriteWizard;
	private AnimationPlayer AnimationPlayer;
	private Sprite2D SpeechBubble;
	public ScriptedFloor ScriptedFloor;
	private bool cutscene_started = false;
	private bool cutscene_finished = false;
	private bool cutscene_cleanup_executed = false;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		
		AnimationPlayer = GetNode<AnimationPlayer>("Transition/AnimationPlayer");
		SpeechBubble = GetNode<Sprite2D>("SpeechBubble");
		var rect = AnimationPlayer.GetParent().GetNode<ColorRect>("ColorRect");
		rect.Color = new Color(0, 0, 0, 1.0f);
		AnimatedSpriteWizard = GetNode<AnimatedSprite2D>("Wizard/AnimatedSprite2D");
		AnimatedSpriteWizard.Play("idle");
		AnimationPlayer.Play("fade_out");
		await ToSignal(AnimationPlayer, "animation_finished");
		
		
		ScriptedFloor = GetNode<ScriptedFloor>("Floor");
		
		//await ToSignal(AnimatedSpriteWizard, AnimatedSprite2D.SignalName.AnimationFinished);
		
	}
	private async void cutscene()
	{
		await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
		AnimatedSpriteWizard.Play("attack");
		await ToSignal(AnimatedSpriteWizard, "animation_finished");
		
		ScriptedFloor.GetTiles(500);
		cutscene_finished=true;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public async override void _Process(double delta)
	{
		if (!cutscene_started)
		{
			cutscene();
			cutscene_started = true;
		}

		if (cutscene_finished && !cutscene_cleanup_executed)
		{
			cutscene_cleanup_executed = true;
			AnimatedSpriteWizard.Play("idle");
			await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
			SpeechBubble.Visible = true;
			await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);
			AnimationPlayer.Play("fade_in");
			await ToSignal(AnimationPlayer, "animation_finished");

			GetTree().ChangeSceneToFile("res://main_scene.tscn");
		}
	}
}
