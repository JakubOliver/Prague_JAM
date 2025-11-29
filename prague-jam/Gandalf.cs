using Godot;
using System;

public partial class Gandalf : Area2D, IPerson, IState
{
	[Export]	
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;
	
	public AnimatedSprite2D AnimatedSprite2D;
	public Floor Floor;

	public PersonState State { get; set; } = PersonState.Idle;

	public string PersonName = "Person";
	
	public int Health { get; set; } = 100;
	 public int Damage { get; set; } = 10;
	
	public double AttackCooldown { get; set; } = 5;
	
	public double HitCooldown { get; set; } = 1.5;

	public double CooldownTimer { get; set; } = 0.0;
	
	public double HitCooldownTimer { get; set; } = 0.0;
	
	public bool InCollision = false;
	public IPerson CollisionVictim = null;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetProcess(true);
		Floor = GetParent().GetNode<Floor>("Floor");
		Speed = 400;
		ScreenSize = GetViewportRect().Size;
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimatedSprite2D.Play("Idle");
		AnimatedSprite2D.AnimationFinished += () =>
		{
			if (State != PersonState.Dead)
			{
				//ChangeState(PersonState.Idle);
				AnimatedSprite2D.Play("Idle");
			}
		};
	}
	public virtual void ChangeState(PersonState newState)
	{
		if (State == PersonState.Dead)
		{
			return;
		}

		if (newState != State)
		{
			GD.Print("Changing state of " + PersonName + " to " + newState);
		}

		OnStateExit(State);
		State = newState;
		OnStateEnter(newState);
	}
	public virtual void OnStateEnter(PersonState state)
	{
		switch (state)
		{
			case PersonState.Idle:
				AnimatedSprite2D.Play("idle");
				break;
			case PersonState.Running:
				AnimatedSprite2D.Play("run");
				break;
			case PersonState.Charging:
				if (CooldownTimer == 0.0)
				{
					CooldownTimer = AttackCooldown;
				}

				break;
			case PersonState.Attack:
				AnimatedSprite2D.Play("attack");
				HitSomeone();
				break;
			case PersonState.Hit:
				AnimatedSprite2D.Play("hit");
				HitCooldownTimer = AttackCooldown;
				
				if (Health <= 0)
				{
					GD.Print("Dead");
					ChangeState(PersonState.Dead);
					return;
				}
				//ChangeState(PersonState.Idle);
				break;
			case PersonState.Dead:
				AnimatedSprite2D.Play("death");
				break;
		}
	}
	public virtual void OnStateExit(PersonState state)
	{
		switch (state)
		{
			case PersonState.Idle:
				break;
			case PersonState.Running:
				break;
			case PersonState.Charging:
				break;
			case PersonState.Attack:
				break;
			case PersonState.Hit:
				break;
			case PersonState.Dead:
				break;
		}
	}
	public virtual void GetHit(int damage, Vector2 scale)
	{
		if (!InCollision || State == PersonState.Dead || State == PersonState.Hit)
		{
			return;
		}

		scale.X = -scale.X;
		Scale = scale;
		
		GD.Print("Hit");
		Health -= damage;
		GD.Print("Health: " + Health);
		ChangeState(PersonState.Hit);
	}
	
	public virtual void HitSomeone()
	{
		if (InCollision)
		{
			CollisionVictim.GetHit(Damage, Scale);
		}
	}
	
	public async void SummonLava()
	{
		AnimatedSprite2D.Play("Attack");
		await ToSignal(AnimatedSprite2D, "animation_finished");
		Floor.GetTiles(5);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CooldownTimer += delta;
		HitCooldownTimer+= delta;
		//Floor.GetTiles(5);
		if (CooldownTimer>=AttackCooldown)
		{
			CooldownTimer =0;
			SummonLava();
			
			
		}
		if (HitCooldownTimer>=HitCooldown)
		{
			HitCooldownTimer =0;
			Floor.ClearLeastRecentLavaTile();
			
			
		}
	}
}
