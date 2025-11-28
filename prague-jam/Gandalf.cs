using Godot;

namespace Praguejam;

public partial class Gandalf : Area2D, IPerson, IMove
{
    public int HP { get; set; } = 250;
    public int Damage { get; set; } = 30;
    
    [Export]	
    public int Speed { get; set; } = 400;
    public Vector2 ScreenSize;

    public PersonClothing head { get; set; } = PersonClothing.Gandalf;
    public PersonClothing body { get; set; } = PersonClothing.Gandalf;
    public PersonClothing weapon { get; set; } = PersonClothing.Gandalf;
    
    public void Move(double delta)
    {
        Vector2 velocity = Vector2.Zero;
        
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