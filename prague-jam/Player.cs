using Godot;
using System;
using System.Diagnostics;



public partial class Player : Person{
	[Signal]
	public delegate void HitEventHandler();

	public void OnBodyEntered(Area2D area)
	{
		if (area is ITile) { 
			GetHit(Health);

			return; 
		}

		InCollision = true;
		InCollisionWith = (Person)area;
	}

	public void OnBodyExited(Area2D area)
	{
		InCollision = false;
		InCollisionWith = null;
	}

	public override void _Ready()
	{
		Health = 100;
		Speed = 400;
		Damage = 25;
		ScreenSize = GetViewportRect().Size;
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimatedSprite2D.Play("idle");
		GetTransition();
	}

	private void ProcessInput(double delta)
	{
		Vector2 direction = Vector2.Zero;
		bool swapSize = false;

		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1;
		} 
		
		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1;
			swapSize = true;
		}

		if (Input.IsActionPressed("move_down"))
		{
			direction.Y += 1;
		} 
		
		if (Input.IsActionPressed("move_up"))
		{
			direction.Y -= 1;
		}

		if (swapSize)
		{
			Scale = new Vector2(-1, 1);
		}
		else
		{
			Scale = new Vector2(1, 1);
		}

		if (Input.IsActionPressed("attack") || Stage == Stages.Attack)
		{	
			TimeInAttack += delta;
			if (TimeInAttack >= ATTACK_TIME_MAX)
			{
				GD.Print("Attack");
				DoHit(InCollisionWith);
				TimeInAttack = 0;
				AttackCoolDown = ATTACK_COOLDOWN_MAX;
			}

			return;
		}
		

		Vector2 velocity = direction.Normalized() * Speed;

		if (velocity != Vector2.Zero)
		{
			ChangeAnimation(Stages.Run);

			Position += velocity * (float)delta;
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(Position.Y, ScreenSize.Y - 900, ScreenSize.Y - 220)
			);
		}
		else
		{
			ChangeAnimation(Stages.Idle);
		}
	}

	public override void _Process(double delta)
	{
		if (Stage == Stages.Hit)
		{
			ProcessHitCoolDown(delta);
		}

		if (Stage == Stages.Dead || Stage == Stages.Hit){ return; }

		ProcessInput(delta);
	}
}
