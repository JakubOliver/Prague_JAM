using Godot;
using System;

public partial class Wizard : Person
{
	Floor floor;

	private const double SUMMON_COOLDOWN_MAX = 5.0;
	private double SummonCoolDown;

	private const double FAIT_COOLDOWN_MAX = 1.5;
	private double FaitCoolDown;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Health = 5;
		Speed = 0;
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimatedSprite2D.Play("idle");
		GetTransition();
		GetDeathSound();
		GetHitSound();

		floor = GetParent().GetNode<Floor>("Floor");

		SummonCoolDown = SUMMON_COOLDOWN_MAX;
		FaitCoolDown = FAIT_COOLDOWN_MAX;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Stage == Stages.Dead)
		{
			return;
		}

		SummonCoolDown -= delta;
		FaitCoolDown -= delta;

		if (SummonCoolDown <= 0)
		{
			AnimatedSprite2D.Play("attack");
			SummonCoolDown = SUMMON_COOLDOWN_MAX;
			AnimatedSprite2D.AnimationFinished += () =>
			{
				for (int i = 0; i < 5; ++i)
				{
					floor.GenerateRandomLavaTile();
				}
				if (Stage == Stages.Dead) return;
				AnimatedSprite2D.Play("idle");
			};
			
		}

		if (FaitCoolDown <= 0)
		{
			floor.ClearLeastRecentLavaTile();
			FaitCoolDown = FAIT_COOLDOWN_MAX;
		}
	}

	protected override async void Dead()
	{
		await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);
		AnimationPlayer.Play("fade_in");
		await ToSignal(AnimationPlayer, "animation_finished");

		GetTree().ChangeSceneToFile("res://win_scene.tscn");
	}
}
