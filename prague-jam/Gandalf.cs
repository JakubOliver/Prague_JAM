using Godot;
using System;

public partial class Gandalf : Person, IPerson, IState
{
[Export]
public int Speed { get; set; } = 400;
public Vector2 ScreenSize;


public AnimatedSprite2D AnimatedSprite2D;
public Floor Floor;

public PersonState State { get; set; } = PersonState.Idle;

public string PersonName = "Gandalf";

public int Health { get; set; } = 10;
public int Damage { get; set; } = 10;

public double AttackCooldown { get; set; } = 5;
public double HitCooldown { get; set; } = 1.5;

public double CooldownTimer { get; set; } = 0.0;
public double HitCooldownTimer { get; set; } = 0.0;

// Called when the node enters the scene tree for the first time.
public override void _Ready()
{
	SetProcess(true);
	Floor = GetParent().GetNode<Floor>("Floor");
	ScreenSize = GetViewportRect().Size;
	AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	AnimatedSprite2D.Play("Idle");
	AnimatedSprite2D.AnimationFinished += () =>
	{
		if (State != PersonState.Dead && State != PersonState.Hit)
		{
			AnimatedSprite2D.Play("Idle");
		}
	};
}

public virtual void ChangeState(PersonState newState)
{
	if (State == PersonState.Dead)
		return;

	if (newState != State)
		GD.Print("Changing state of " + PersonName + " to " + newState);

	OnStateExit(State);
	State = newState;
	OnStateEnter(newState);
}

public virtual void OnStateEnter(PersonState state)
{
	switch (state)
	{
		case PersonState.Idle:
			AnimatedSprite2D.Play("Idle");
			break;
		case PersonState.Running:
			AnimatedSprite2D.Play("Run");
			break;
		case PersonState.Charging:
			if (CooldownTimer == 0.0)
				CooldownTimer = AttackCooldown;
			break;
		case PersonState.Attack:
			AnimatedSprite2D.Play("Attack");
			HitSomeone();
			break;
		case PersonState.Hit:
			AnimatedSprite2D.Play("Hit");
			HitCooldownTimer = HitCooldown;
			break;
		case PersonState.Dead:
			AnimatedSprite2D.Play("Death");
			break;
	}
}

public virtual void OnStateExit(PersonState state) { }

public virtual void GetHit(int damage, Vector2 scale)
{
	if (State == PersonState.Dead || State == PersonState.Hit)
		return;


	//scale.X = -Mathf.Sign(scale.X);
	//Scale = scale;

	Health -= damage;
	GD.Print(PersonName + " Health: " + Health);

	if (Health <= 0)
	{
		ChangeState(PersonState.Dead);
	}
	else
	{
		ChangeState(PersonState.Hit);
	}
}

public virtual void HitSomeone()
{
	if (CollisionVictim != null && CollisionVictim.State != PersonState.Dead)
	{
		CollisionVictim.GetHit(Damage, Scale);
	}
}

public async void SummonLava()
{
	ChangeState(PersonState.Attack);
	AnimatedSprite2D.Play("Attack");
	await ToSignal(AnimatedSprite2D, "animation_finished");
	Floor.GetTiles(5);
	ChangeState(PersonState.Idle);
}

// Called every frame. 'delta' is the elapsed time since the previous frame.
public override void _Process(double delta)
{
	// Cooldown pro útok
	if (State != PersonState.Dead)
	{
		if (CooldownTimer < AttackCooldown)
			CooldownTimer += delta;

		if (HitCooldownTimer < HitCooldown)
			HitCooldownTimer += delta;

		if (CooldownTimer >= AttackCooldown)
		{
			CooldownTimer = 0;
			SummonLava();
		}

		if (HitCooldownTimer >= HitCooldown)
		{
			HitCooldownTimer = 0;
			Floor.ClearLeastRecentLavaTile();
		}
	}
}


}
