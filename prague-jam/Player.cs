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
	void GetHit(int damage);
	void HitSomeone();
}

public enum PersonState
{
	Idle, Running, Dead, Hit, Attack, Charging
}

public partial class Player : Area2D, IPerson, IState
{
	[Export]	
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;

	public PersonState State { get; set; } = PersonState.Idle;
	
	public int Health { get; set; } = 100;
	public int Damage { get; set; } = 10;

	public void OnStateEnter(PersonState state)
	{
		switch (state)
		{
			case PersonState.Idle:
				_animatedSprite2D.Play("idle");
				break;
			case PersonState.Running:
				_animatedSprite2D.Play("run");
				break;
			case PersonState.Charging:
				ChangeState(PersonState.Attack);
				break;
			case PersonState.Attack:
				HitSomeone();
				_animatedSprite2D.Play("attack");
				break;
			case PersonState.Hit:
				_animatedSprite2D.Play("hit");
				ChangeState(PersonState.Idle);
				break;
			case PersonState.Dead:
				_animatedSprite2D.Play("death");
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

	private bool _inCollision = false;
	private IPerson _collisionVictim = null;
	
	private void OnBodyEntered(Node2D body)
	{
		_inCollision = true;
		_collisionVictim = (IPerson)body;
		//ChangeState(PersonState.Charging);
	}
	
	private void OnBodyExited(Node2D body) {
		_inCollision = false;
		_collisionVictim = null;
		ChangeState(PersonState.Idle);
	}
	
	public void HitSomeone()
	{
		if (_inCollision)
		{
			_collisionVictim.GetHit(Damage);
		}
	}
	
	public void GetHit(int damage)
	{
		if (!_inCollision)
		{
			return;
			
		}
		
		Health -= damage;
		ChangeState(PersonState.Hit);
		
	}

	public void Start(Vector2 position)
	{
		Position = position;
	}
	
	private AnimatedSprite2D _animatedSprite2D;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite2D.Play("idle");
		_animatedSprite2D.AnimationFinished += () =>
		{
			ChangeState(PersonState.Idle);
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (State == PersonState.Dead)
		{
			return;
		}
		
		Vector2 velocity = Vector2.Zero;
		Vector2 scale = new Vector2(1, 1);

		if (State != PersonState.Hit)
		{
			if (Input.IsActionPressed("move_right"))
			{
				velocity.X += 1;
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
			ChangeState(PersonState.Charging);
		}
		
		if (velocity.Length() > 0){
			velocity = velocity.Normalized()*Speed;
		}


		if (State != PersonState.Attack)
		{
			if (velocity != Vector2.Zero)
			{
				ChangeState(PersonState.Running);
				_animatedSprite2D.Play("run");
			}
			else
			{
				ChangeState(PersonState.Idle);
				_animatedSprite2D.Play("idle");
			}
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		Scale = scale;
	}

}
