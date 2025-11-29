using Godot;
using System;

public interface IState
{
	PersonState State { get; set; }
	void OnStateEnter(PersonState state);
	void OnStateExit(PersonState state);
}

public interface IPerson
{
	int Health { get; set; }
	int Damage { get; set; }
	PersonState State { get; set; }
	void ChangeState(PersonState newState);
	void GetHit(int damage, Vector2 scale);
	void HitSomeone();
}

public enum PersonState
{
	Idle, Running, Dead, Hit, Attack, Charging
}

public abstract partial class Person : Area2D, IPerson, IState
{
	[Export]	
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;
	
	public AnimatedSprite2D AnimatedSprite2D;

	public PersonState State { get; set; } = PersonState.Idle;

	public string PlayerName = "Person";
	
	public int Health { get; set; } = 100;
	public int Damage { get; set; } = 10;
	
	public double AttackCooldown { get; set; } = 0.1;
	
	public double HitCooldown { get; set; } = 0.2;

	public double CooldownTimer { get; set; } = 0.0;
	
	public double HitCooldownTimer { get; set; } = 0.0;
	
	public bool InCollision = false;
	public IPerson CollisionVictim = null;
	
	public virtual void ChangeState(PersonState newState)
	{
		if (State == PersonState.Dead)
		{
			return;
		}

		if (newState != State)
		{
			GD.Print("Changing state of " + PlayerName + " to " + newState);
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
	
	public virtual void HitSomeone()
	{
		if (InCollision)
		{
			CollisionVictim.GetHit(Damage, Scale);
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
	
	public virtual void Start(Vector2 position)
	{
		Position = position;
	}

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimatedSprite2D.Play("idle");
		AnimatedSprite2D.AnimationFinished += () =>
		{
			if (State != PersonState.Dead)
			{
				ChangeState(PersonState.Idle);
				AnimatedSprite2D.Play("idle");
			}
		};
	}
}

public partial class Player : Person
{
	[Signal]
	public delegate void HitEventHandler();

	private void OnBodyEntered(Node2D body)
	{
		InCollision = true;
		if (body is IPerson)
		{
			CollisionVictim = (IPerson)body;
		//ChangeState(PersonState.Charging);
		}

		if (body is ITile)
		{
			GD.Print("Collided with title");
			GetHit(Health, Scale);

			GD.Print(Health);
			return;
		}
	}
	
	private void OnBodyExited(Node2D body) {
		InCollision = false;
		CollisionVictim = null;
		ChangeState(PersonState.Idle);
	}
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		PlayerName = "Player";
	}
	
	Vector2 scale = new Vector2(1, 1);

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (State == PersonState.Dead)
		{
			return;
		}
		
		Vector2 velocity = Vector2.Zero;
		

		if (State != PersonState.Hit)
		{
			if (Input.IsActionPressed("move_right"))
			{
				velocity.X += 1;
				scale.X = 1;
			}
		
			if (Input.IsActionPressed("move_left"))
			{
				velocity.X -= 1;
				scale.X = -1;
			}

			if (Input.IsActionPressed("move_down"))
			{
				velocity.Y += 1;
			}

			if (Input.IsActionPressed("move_up"))
			{
				velocity.Y -= 1;
			}
		}
		
		if (Input.IsActionPressed("attack"))
		{
			GD.Print("Attack");
			ChangeState(PersonState.Charging);
		}
		
		if (velocity.Length() > 0){
			velocity = velocity.Normalized()*Speed;
		}


		if (State != PersonState.Attack && State != PersonState.Charging)
		{
			if (velocity != Vector2.Zero)
			{
				ChangeState(PersonState.Running);
				AnimatedSprite2D.Play("run");
			}
			else
			{
				ChangeState(PersonState.Idle);
				AnimatedSprite2D.Play("idle");
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

		if (State != PersonState.Charging && State != PersonState.Attack)
		{
			Position += velocity * (float)delta;
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(Position.Y, ScreenSize.Y - 900, ScreenSize.Y)
			);
		}
		
		Scale = scale;
		
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
