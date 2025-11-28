using Godot;
using System;

public partial class Player : Area2D, IPerson, IMove
{
	[Export]	
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;

	public int HP { get; set; } = 100;
	public int Damage { get; set; } = 10;

	public PersonClothing head { get; set; } = PersonClothing.Player;
	public PersonClothing body { get; set; } = PersonClothing.Player;
	public PersonClothing weapon { get; set; } = PersonClothing.Player;

	public void Move(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}
		
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}
		
		if (velocity.Length() > 0){
			velocity = velocity.Normalized()*Speed;
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
	}

	public void Start(Vector2 position)
	{
		Position = position;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Move(delta);

		GD.Print(Position);
	}
}
