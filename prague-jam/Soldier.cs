using Godot;
using System;



public partial class Soldier : Area2D, IPerson
{
	[Signal]
	public delegate void HitEventHandler();

	private AnimatedSprite2D _animatedSprite2D;
	
	public int Health { get; set; } = 150;
	public int Damage { get; set; } = 20;
	
	public void ChangeState(PersonState newState)
	{
		State = newState;
	}
	
	public PersonState State { get; set; } = PersonState.Idle;

	public double Cooldown { get; set; } = 0.2;

	public double CooldownTimer { get; set; } = 0.0;
	
	
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

	public void GetHit(int damage)
	{
		Health -= damage;
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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (CooldownTimer > 0 && State == PersonState.Charging)
		{
			if (CooldownTimer - delta <= 0)
			{
				CooldownTimer = 0;
				_animatedSprite2D.Play("attack");
				_animatedSprite2D.AnimationFinished += () =>
				{
					_animatedSprite2D.Play("idle");
				};
			}
			else
			{
				CooldownTimer -= delta;
			}
		}
		
		
	}
}
