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
	Idle, Running, Dead, Hit, Attack, Charging, None
}

public class DoubleWrapper
{
	public double Value;
	
	public DoubleWrapper(double value)
	{
		Value = value;
	}
}

public abstract partial class Person : Area2D, IPerson, IState
{
	[Signal]
	public delegate void HitEventHandler();
	
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;
	
	public Sprite2D Sprite2D;
	public AnimationTree AnimationTree;

	public PersonState State { get; set; } = PersonState.Idle;

	public string PersonName = "Person";
	
	public int Health { get; set; } = 100;
	public int Damage { get; set; } = 10;
	
	public double AttackCooldown { get; set; } = 0.3;
	
	public double HitCooldown { get; set; } = 0.2;

	public DoubleWrapper CooldownTimer { get; set; } = new DoubleWrapper(0.0);
	
	public DoubleWrapper HitCooldownTimer { get; set; } = new DoubleWrapper(0.0);
	
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
				//AnimatedSprite2D.Play("idle");
				break;
			case PersonState.Running:
				//AnimatedSprite2D.Play("run");
				break;
			case PersonState.Charging:
				break;
			case PersonState.Attack:
				//AnimatedSprite2D.Play("attack");
				if (CooldownTimer.Value == 0.0)
				{
					CooldownTimer.Value = AttackCooldown;
				}
				break;
			case PersonState.Hit:
				//AnimatedSprite2D.Play("hit");
				HitCooldownTimer.Value = HitCooldown;
				
				if (Health <= 0)
				{
					GD.Print("Dead");
					ChangeState(PersonState.Dead);
					return;
				}
				//ChangeState(PersonState.Idle);
				break;
			case PersonState.Dead:
				ZIndex = 1;
				//AnimatedSprite2D.Play("death");
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
	
	public void CoolDownManager(double delta, DoubleWrapper cooldown, Action onComplete)
	{
		if (cooldown.Value > 0)
		{
			cooldown.Value -= delta;
		}
		if (cooldown.Value <= 0)
		{
			GD.Print("Invoke");
			cooldown.Value = 0;
			onComplete?.Invoke();
		}
	}
	
	public virtual void HitSomeone()
	{
		GD.Print("Hit once");
		if (CollisionVictim == null)
		{
			return;
		}
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

	public void UpdateAnimationParameters()
	{
		AnimationTree.Set("parameters/conditions/IsAttacking", State == PersonState.Attack);
		AnimationTree.Set("parameters/conditions/IsIdle", State == PersonState.Idle);
		AnimationTree.Set("parameters/conditions/IsRunning", State == PersonState.Running);
		AnimationTree.Set("parameters/conditions/IsHit", State == PersonState.Hit);
		AnimationTree.Set("parameters/conditions/IsDead", State == PersonState.Dead);
	}
	
	public virtual void Start(Vector2 position)
	{
		Position = position;
	}

	public AnimationPlayer AnimationPlayer;

	public override void _Ready()
	{
		Speed = 400;
		ScreenSize = GetViewportRect().Size;
		Sprite2D = GetNode<Sprite2D>("Sprite2D");
		AnimationTree = GetNode<AnimationTree>("AnimationTree");
		AnimationTree.Active = true;
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationPlayer.SpeedScale = 3;
		//AnimatedSprite2D.Play("idle");
	}
}

public partial class Player : Person
{
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
		PersonName = "Player";
	}
	
	Vector2 scale = new Vector2(1, 1);

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateAnimationParameters();
		
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
			ChangeState(PersonState.Attack);
		}
		
		if (velocity.Length() > 0){
			velocity = velocity.Normalized()*Speed;
		}


		if (State != PersonState.Attack)
		{
			if (velocity != Vector2.Zero)
			{
				ChangeState(PersonState.Running);
			}
			else
			{
				ChangeState(PersonState.Idle);
			}
		}
		
		if (HitCooldownTimer.Value > 0)
		{
			CoolDownManager(delta, HitCooldownTimer, () =>
			{
				HitCooldownTimer.Value = 0;
				ChangeState(PersonState.Idle);
			});
		}
		
		

		if (State != PersonState.Charging && State != PersonState.Attack)
		{
			Position += velocity * (float)delta;
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(Position.Y, ScreenSize.Y - 900, ScreenSize.Y - 220)
			);
		}
		
		Scale = scale;

		if (CooldownTimer.Value > 0)
		{
			CoolDownManager(delta, CooldownTimer, () =>
			{
				CooldownTimer.Value = 0;
				HitSomeone();
				//ChangeState(PersonState.Attack);
				ChangeState(PersonState.Idle);
			});
		}
		
		
		
	}

}
