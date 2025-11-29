using Godot;
using System;
using System.Collections.Generic;

public partial class Floor : TileMapLayer
{
	private const int MaxNotOutOfRangeCoordinateX = 113;
	private const int MaxNotOutOfRangeCoordinateY = 38;
	
	private readonly Queue<Vector2I> _lavaTiles = new();

	private Vector2 ArenaCoordinates(Vector2I floorCoordinates)
	{
		var tileMapPosition = Position;
		return tileMapPosition + floorCoordinates * TileSet.TileSize;
	}
	
	private void SetLavaTile(Vector2I floorCoordinates)
	{
		SetCell(floorCoordinates, 0, new Vector2I(0, 0));
		_lavaTiles.Enqueue(floorCoordinates);
	}

	private bool OverlapsWithPlayer(Vector2 arenaCoordinates)
	{
		const int lavaBlockSize = 100;
		const int playerSize = 230;
		
		var player = GetParent().GetNode<Player>("Player");
		var collisionShape = player.GetNode<CollisionShape2D>("CollisionShape2D");
		
		var playerPosition = player.Position - 150 * new Vector2(1, 0);
	
		Rect2 playerRect = new Rect2(playerPosition, new Vector2(playerSize, playerSize));
		Rect2 tileRect = new Rect2(arenaCoordinates, new Vector2(lavaBlockSize, lavaBlockSize));
		
		return playerRect.Intersects(tileRect);
	}
	
	
	
	public void GenerateRandomLavaTile()
	{
		int x = GD.RandRange(0, MaxNotOutOfRangeCoordinateX);
		int y = GD.RandRange(0, MaxNotOutOfRangeCoordinateY);
		
		var floorCoordinates = new Vector2I(x, y);
		
		if (OverlapsWithPlayer(ArenaCoordinates(floorCoordinates))) return;
		SetLavaTile(floorCoordinates);
	}
	public void ClearLeastRecentLavaTile()
	{
		if (_lavaTiles.TryDequeue(out var coordinates))
			EraseCell(coordinates);
	}
	
	public void CoverWithLava() {
		for (int x = 0; x <= MaxNotOutOfRangeCoordinateX; ++x)
		for (int y = 0; y <= MaxNotOutOfRangeCoordinateY; ++y)
			SetLavaTile(new Vector2I(x, y));
	}	
	
	public override void _Ready()
	{

		// GD.Seed(12343);
		for (int i = 0; i < 5; ++i) GenerateRandomLavaTile();
		//for (int i = 0; i < 5; ++i) ClearLeastRecentLavaTile();
	}
}
