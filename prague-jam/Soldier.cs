using System;
using Godot;

public partial class Soldier : Area2D, IPerson, IMove
{
    public int HP { get; set; } = 150;
    public int Damage { get; set; } = 20;

    private Random _rng = new Random();
    
    [Export]	
    public int Speed { get; set; } = 400000;
    public Vector2 ScreenSize;

    public PersonClothing head { get; set; } = PersonClothing.Soldier;
    public PersonClothing body { get; set; } = PersonClothing.Soldier;
    public PersonClothing weapon { get; set; } = PersonClothing.Soldier;

    public void Move(double delta)
    {
        Vector2 velocity = Vector2.Zero;
        int dir1 = _rng.Next(0, 2);
        int dir2 = _rng.Next(0, 2);
        velocity.X = dir1 == 1 ? Speed : -Speed;
        velocity.Y = dir2 == 1 ? Speed : -Speed;
        
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