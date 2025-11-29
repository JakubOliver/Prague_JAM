using Godot;
using System;

public partial class intro_cutscene : Node2D
{
	public AnimatedSprite2D AnimatedSpriteWizard;
	public ScriptedFloor ScriptedFloor;
	private bool cutscene_started = false;
	private bool cutscene_finished = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		AnimatedSpriteWizard = GetNode<AnimatedSprite2D>("Wizard/AnimatedSprite2D");
		ScriptedFloor = GetNode<ScriptedFloor>("Floor");
		
		AnimatedSpriteWizard.Play("Idle");
		
		//await ToSignal(AnimatedSpriteWizard, AnimatedSprite2D.SignalName.AnimationFinished);
		
	}
	private async void cutscene()
	{
		await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
		AnimatedSpriteWizard.Play("Attack");
		await ToSignal(AnimatedSpriteWizard, "animation_finished");
		
		ScriptedFloor.GetTiles(500);
		cutscene_finished=true;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!cutscene_started)
		{
			cutscene();
			cutscene_started = true;
		}
		if (cutscene_finished)
		{
			AnimatedSpriteWizard.Play("Idle");
		}
	}
}
