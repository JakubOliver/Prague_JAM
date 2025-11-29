using Godot;
using System;



public partial class Soldier : Person
{
	[Signal]
	public delegate void HitEventHandler();

	public Player PlayerInstance;

	public double LastDirectionChange = 0.2;
	public DoubleWrapper LastDirectionChangeCooldown = new DoubleWrapper(0.0);
	public bool CanChangeDirection = true;

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
		//if (State != PersonState.Hit)
		//{
			ChangeState(PersonState.Idle);
		//}
	}
	
	public override void OnStateEnter(PersonState state)
	{
		base.OnStateEnter(state);
		switch (state)
		{
			case PersonState.Attack:
				if (CollisionVictim == null)
				{
					return;
				}
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
		Damage = 1;
		
		Speed = 250;
		PersonName = "Soldier";
		
		//PlayerInstance = GetNode<Area2D>("Player");
		PlayerInstance = (Player)GetParent().GetNode<Area2D>("Player");
		
		AttackCooldown = 0.8;
		
		ScreenSize = GetViewportRect().Size;
		
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		//AnimatedSprite2D.SetAnimation("idle");
		ChangeState(PersonState.Idle);
		/*AnimatedSprite2D.AnimationFinished += () =>
		{
			if (State != PersonState.Dead && State != PersonState.Attack && State != PersonState.Charging)
			{
				AnimatedSprite2D.Play("idle");
			}
		};*/
	}

	private Vector2 velocityBase = new Vector2(1, 1);

	public void Move(double delta)
	{

		if (PlayerInstance.State != PersonState.Dead)
		{
			GD.Print(PlayerInstance.Position);
			Vector2 diff = PlayerInstance.Position - Position;
			if (!InCollision)
			{
				if (CanChangeDirection)
				{
					GD.Print("Dir changed");
					velocityBase.X = diff.X > 1 ? 1 : diff.X == 0 ? 0 : -1;
					velocityBase.Y = diff.Y > 1 ? 1 : diff.Y == 0 ? 0 : -1;
					CanChangeDirection = false;
					LastDirectionChangeCooldown.Value = LastDirectionChange;
				}
				
			}
		}

		Vector2 velocity = velocityBase;
		
		if (State != PersonState.Attack && State != PersonState.Charging)
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
		
		if (velocity.Length() > 0){
			velocity = velocity.Normalized() * Speed;
		}

		if (PlayerInstance.State == PersonState.Dead)
		{
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
							x: ScreenSize.X - 10,
							y: Position.Y
						);
					}

					velocityBase.X = -velocityBase.X;
					velocityBase.Y = velocityBase.Y;
				}
			}
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X), 
				y: Mathf.Clamp(Position.Y, ScreenSize.Y - 900, ScreenSize.Y - 220)
			);

		Vector2 scale = new Vector2(velocityBase.X, 1);

		Scale = scale;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (State == PersonState.Dead)
		{
			return;
		}
		
		Move(delta);
		
		if (CooldownTimer.Value > 0)
		{
			CoolDownManager(delta, CooldownTimer, () =>
			{
				CooldownTimer.Value = 0;
				ChangeState(PersonState.Attack);
			});
		}
		
		if (HitCooldownTimer.Value > 0)
		{
			CoolDownManager(delta, HitCooldownTimer, () =>
				{
					HitCooldownTimer.Value = 0;
					if (InCollision && CollisionVictim.State != PersonState.Dead)
					{
						GD.Print("Changed to charging");
						//AnimatedSprite2D.Play("idle");
						ChangeState(PersonState.Charging);
						return;
					}

					GD.Print("Hit cooldown?");
					ChangeState(PersonState.Idle);
				});
		}

		if (LastDirectionChangeCooldown.Value > 0)
		{
			CoolDownManager(delta, LastDirectionChangeCooldown, () =>
			{
				CanChangeDirection = true;
			});
		}
		
		
		
	}
}
