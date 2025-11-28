using Godot;
using System;

public partial class Player : Area2D
{
	[Export]	
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;

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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero;
		Vector2 scale = new Vector2(1, 1); 

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
		
		if (Input.IsActionPressed("attack"))
		{
			changeToAttack("idle");
		}
		
		if (velocity.Length() > 0){
			velocity = velocity.Normalized()*Speed;
		}


		if (!(_animatedSprite2D.GetAnimation() == "attack"))
		{
			if (velocity != Vector2.Zero)
			{
				_animatedSprite2D.Play("run");
			}
			else
			{
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

	void changeToAttack(String next)
	{
		_animatedSprite2D.Play("attack");
		_animatedSprite2D.AnimationFinished += () =>
		{
			_animatedSprite2D.Play(next);
		};
	}
}
