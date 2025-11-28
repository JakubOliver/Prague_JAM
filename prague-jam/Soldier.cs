using Godot;
using System;



public partial class Soldier : Area2D, IPerson, IState
{
	[Signal]
	public delegate void HitEventHandler();

	private AnimatedSprite2D _animatedSprite2D;
	
	public int Health { get; set; } = 150;
	public int Damage { get; set; } = 20;
	
	public void ChangeState(PersonState newState)
	{
		if (State == PersonState.Dead)
		{
			return;
		}
		OnStateExit(State);
		State = newState;
		OnStateEnter(newState);
	}
	
	public PersonState State { get; set; } = PersonState.Idle;

	public double Cooldown { get; set; } = 0.2;

	public double CooldownTimer { get; set; } = 0.0;
	
	public double HitCooldownTimer { get; set; } = 0.0;
	
	
	private bool _inCollision = false;
	private IPerson _collisionVictim = null;
	
	private void OnBodyEntered(Node2D body) {
		_inCollision = true;
		_collisionVictim = (IPerson)body;
		CooldownTimer = Cooldown;
		ChangeState(PersonState.Charging);
	}
	
	private void OnBodyExited(Node2D body) {
		_inCollision = false;
		_collisionVictim = null;
		ChangeState(PersonState.Idle);
	}
	
	public void OnStateEnter(PersonState state)
	{
		switch (state)
		{
			case PersonState.Idle:
				GD.Print("Changed to idle!");
				_animatedSprite2D.Play("idle");
				break;
			case PersonState.Running:
				_animatedSprite2D.Play("run");
				break;
			case PersonState.Charging:
				break;
			case PersonState.Attack:
				_animatedSprite2D.Play("attack");
				break;
			case PersonState.Hit:
				_animatedSprite2D.Play("hit");
				
				HitCooldownTimer = Cooldown;
				
				if (Health <= 0)
				{
					ChangeState(PersonState.Dead);
					return;
				}
				GD.Print("Change");
				break;
			case PersonState.Dead:
				_animatedSprite2D.Play("death");
				break;
		}
	}
	
	public void OnStateDuration(double delta)
	{
		switch (State)
		{
			case PersonState.Idle:
				_animatedSprite2D.Play("idle");
				break;
			case PersonState.Running:
				_animatedSprite2D.Play("run");
				break;
			case PersonState.Charging:
				break;
			case PersonState.Attack:
				_animatedSprite2D.Play("attack");
				break;
			case PersonState.Hit:
				break;
			case PersonState.Dead:
				break;
		}
	}
	
	public void OnStateExit(PersonState state)
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

	public void GetHit(int damage)
	{
		if (!_inCollision || State == PersonState.Dead || State == PersonState.Hit)
		{
			return;
		}
		
		GD.Print("Hit");
		Health -= damage;
		GD.Print("Health: " + Health);
		ChangeState(PersonState.Hit);
	}
	
	public void HitSomeone()
	{
		if (_inCollision)
		{
			_collisionVictim.GetHit(Damage);
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite2D.Play("idle");
		_animatedSprite2D.AnimationFinished += () =>
		{
			if (State != PersonState.Dead)
			{
				_animatedSprite2D.Play("idle");
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
				_animatedSprite2D.Play("attack");
				
				
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
				ChangeState(PersonState.Idle);
			}
			else
			{
				HitCooldownTimer -= delta;
			}
		}
		
	}
}


