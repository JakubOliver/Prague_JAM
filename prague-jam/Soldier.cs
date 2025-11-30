using Godot;
using System;



public partial class Soldier : Person
{
	Player playerArea;

	public const double AI_ATTACK_TIME_MAX = 1.5;
	public double AIAttackTime = 0.0;

	public override void _Ready()
	{
		Health = 200;
		Speed = 200;
		Damage = 20;

		playerArea = GetParent().GetNode<Player>("Player");
		InCollisionWith = playerArea;
		ScreenSize = GetViewportRect().Size;
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimatedSprite2D.Play("idle");
		GetTransition();
		GetDeathSound();
		GetHitSound();
	}

	public void OnBodyEntered(Area2D area)
	{
		if (area is ITile or Wizard) { 
			return; 
		}

		InCollision = true;
		GD.Print("Collision with soldier");

		AIAttackTime = AI_ATTACK_TIME_MAX / 2;
	}

	public void OnBodyExited(Area2D area)
	{
		InCollision = false;
	}

	private void ProcessAI(double delta)
	{
		if (InCollision || Stage == Stages.Attack	)
		{
			AIAttackTime -= delta;

			if (AIAttackTime <= 0)
			{
				DoHit(playerArea);
				AIAttackTime = AI_ATTACK_TIME_MAX;
			}
			return;
		}

		Vector2 direction = (Position - playerArea.Position).Normalized();

		Position -= direction * Speed * (float)delta;

		if (direction.Length() > 0)
		{
			ChangeAnimation(Stages.Run);
		}
		else
		{
			ChangeAnimation(Stages.Idle);
		}

		if (direction.X > 0)
		{
			Scale = new Vector2(-1, 1);
		}
		else
		{
			Scale = new Vector2(1, 1);
		}
	}

	public override void _Process(double delta)
	{
		if (Stage == Stages.Hit)
		{
			ProcessHitCoolDown(delta);
		}

		if (Stage == Stages.Dead || Stage == Stages.Hit){ return; }

		ProcessAI(delta);
	}
}
