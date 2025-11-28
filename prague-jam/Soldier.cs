using Godot;

public partial class Soldier : RigidBody2D, IPerson, IMove
{
	[Signal]
	public delegate void HitEventHandler();
	
	public int HP { get; set; } = 150;
	public int Damage { get; set; } = 20;
	
	[Export]	
	public int Speed { get; set; } = 400;
	public Vector2 ScreenSize;

	public PersonClothing head { get; set; } = PersonClothing.Soldier;
	public PersonClothing body { get; set; } = PersonClothing.Soldier;
	public PersonClothing weapon { get; set; } = PersonClothing.Soldier;

	public void Move(double delta)
	{
		Vector2 Velocity = new Vector2(100, 100);
		
		// Using MoveAndCollide.
		var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null)
		{
			GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
		}
	}
	
	private void OnBodyEntered(Node2D node){
		GD.Print("ahoj");
	}
	
	public void Start(Vector2 position)
	{
		Position = position;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ContactMonitor = true;
		MaxContactsReported = 25;
		ScreenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Move(delta);

		//GD.Print(Position);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		Move(delta);
	}
}
