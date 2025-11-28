using Godot;
using System;



public partial class Soldier : Area2D, IPerson
{
	[Signal]
	public delegate void HitEventHandler();

	private AnimatedSprite2D _animatedSprite2D;
	
	public int Health { get; set; } = 150;
	public int Damage { get; set; } = 20;

	public double Cooldown { get; set; } = 0.2;

	public double CooldownTimer { get; set; } = 0.0;
	
	
	
	private void OnBodyEntered(Node2D body) {
		CooldownTimer = Cooldown;
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
		GD.Print(CooldownTimer);
		if (CooldownTimer > 0)
		{
			if (CooldownTimer - delta <= 0)
			{
				CooldownTimer = 0;
				_animatedSprite2D.Play("attack");
				_animatedSprite2D.AnimationFinished += () =>
				{
					_animatedSprite2D.Play("idle");
				};
				GD.Print("třeba ahoj");
			}
			else
			{
				CooldownTimer -= delta;
			}
		}
		
		
	}
}
