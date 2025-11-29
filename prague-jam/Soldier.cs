using Godot;
using System;



public partial class Soldier : Person
{
	[Signal]
	public delegate void HitEventHandler();

	private void OnBodyEntered(Node2D body)
	{
		if (body is ITile)
		{
			return;
		}

		InCollision = true;
		CollisionVictim = (IPerson)body;
		if (CollisionVictim.State != PersonState.Dead)
		{
			ChangeState(PersonState.Charging);
		}
	}
	
	private void OnBodyExited(Node2D body) {
		GD.Print("OnBodyExited");
		InCollision = false;
		CollisionVictim = null;
		if (State != PersonState.Hit)
		{
			ChangeState(PersonState.Idle);
		}
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
		PersonName = "Soldier";
		
		AttackCooldown = 0.5;
		
		ScreenSize = GetViewportRect().Size;
		
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimatedSprite2D.Play("idle");
		AnimatedSprite2D.SetAnimation("idle");
		AnimatedSprite2D.AnimationFinished += () =>
		{
			if (State != PersonState.Dead && State != PersonState.Attack && State != PersonState.Charging)
			{
				AnimatedSprite2D.Play("idle");
			}
		};
	}

	private Vector2 velocityBase = new Vector2(1, 1);

	public void Move(double delta)
	{
		Vector2 velocity = velocityBase;
		
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
		
		if (velocity.Length() > 0){
			velocity = velocity.Normalized() * Speed;
		}
		GD.Print("Screensize: " + ScreenSize);
		if (State != PersonState.Charging && State != PersonState.Attack)
		{
			if (Position.Y >= ScreenSize.Y - 220 || Position.Y <= ScreenSize.Y - 900)
			{
				if (Position.Y >= ScreenSize.Y - 220)
				{
					Position = new Vector2(
						x: Position.X,
						y: ScreenSize.Y - 230
						);
				}
				if (Position.Y <= ScreenSize.Y - 900)
				{
					Position = new Vector2(
						x: Position.X,
						y: ScreenSize.Y - 890
					);
				}
				velocityBase.X = velocityBase.X;
				velocityBase.Y = -velocityBase.Y;
			}

			if (Position.X <= 0 || Position.X >= ScreenSize.X)
			{
				if (Position.X <= 0)
				{
					Position = new Vector2(
						x: 10,
						y: Position.Y
					);
				}

				if (Position.X >= ScreenSize.X)
				{
					Position = new Vector2(
						x: ScreenSize.X-10,
						y: Position.Y
					);
				}
				
				velocityBase.X = -velocityBase.X;
				velocityBase.Y = velocityBase.Y;
			}
			
			Position += velocity * (float)delta;
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(Position.Y, ScreenSize.Y - 900, ScreenSize.Y - 220)
			);

			Vector2 scale = new Vector2(velocityBase.X, 1);
			
			Scale = scale;
		}


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (State == PersonState.Dead)
		{
			return;
		}
		
		Move(delta);
		
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
