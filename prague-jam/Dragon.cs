using Godot;

public partial class Dragon : Area2D, IPerson, IMove
{
    public int HP { get; set; } = 300;
    public int Damage { get; set; } = 50;
    
    [Export]	
    public int Speed { get; set; } = 400;
    public Vector2 ScreenSize;

    public PersonClothing head { get; set; } = PersonClothing.Dragon;
    public PersonClothing body { get; set; } = PersonClothing.Dragon;
    public PersonClothing weapon { get; set; } = PersonClothing.Dragon;

    public void Move(double delta)
    {
        throw new System.NotImplementedException();
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