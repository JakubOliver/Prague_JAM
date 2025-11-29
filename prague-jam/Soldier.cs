using Godot;
using System;



public partial class Soldier : Person
{
	[Signal]
	public delegate void HitEventHandler();
	
	private void OnBodyEntered(Node2D body) {
		InCollision = true;
		CollisionVictim = (IPerson)body;
		ChangeState(PersonState.Charging);
	}
	
	private void OnBodyExited(Node2D body) {
		GD.Print("OnBodyExited");
		InCollision = false;
		CollisionVictim = null;
	}
	
	public override void OnStateEnter(PersonState state)
	{
		base.OnStateEnter(state);
		switch (state)
		{
			case PersonState.Attack:
				if (CollisionVictim.State != PersonState.Dead)
				{
					ChangeState(PersonState.Charging);
				}
				else
				{
					ChangeState(PersonState.Idle);
				}

				break;
		}
	}
	
	public override void HitSomeone()
	{
		if (InCollision && CollisionVictim.State != PersonState.Dead)
		{
			CollisionVictim.GetHit(Damage, Scale);
		}
		else
		{
			ChangeState(PersonState.Idle);
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Name = "Soldier";
		
		AttackCooldown = 0.5;
		
		
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimatedSprite2D.Play("idle");
		AnimatedSprite2D.SetAnimation("idle");
		AnimatedSprite2D.AnimationFinished += () =>
		{
			if (State != PersonState.Dead && State != PersonState.Attack && State != PersonState.Charging)
			{
				GD.Print("Idle again huh");
				AnimatedSprite2D.Play("idle");
			}
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (State == PersonState.Dead)
		{
			return;
		}
		
		if (CooldownTimer > 0 && State == PersonState.Charging)
		{
			if (CooldownTimer - delta <= 0)
			{
				CooldownTimer = 0;
				ChangeState(PersonState.Attack);
			}
			else
			{
				CooldownTimer -= delta;
			}
		}
		
		if (HitCooldownTimer > 0)
		{
			if (HitCooldownTimer - delta <= 0)
			{
				HitCooldownTimer = 0;
				if (InCollision && CollisionVictim.State != PersonState.Dead)
				{
					GD.Print("Changed to charging");
					AnimatedSprite2D.Play("idle");
					ChangeState(PersonState.Charging);
					return;
				}

				GD.Print("Hit cooldown?");
				ChangeState(PersonState.Idle);
			}
			else
			{
				HitCooldownTimer -= delta;
			}
		}
		
	}
}
