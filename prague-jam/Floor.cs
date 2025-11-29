using Godot;
using System;
using System.Collections.Generic;

public partial class Floor : TileMapLayer
{
	const int _maxNotOutOfRangeCoordinateX = 113;
	const int _maxNotOutOfRangeCoordinateY = 38;
	
	private readonly Queue<Vector2I> _lavaTiles = new();

	private void SetLavaTile(Vector2I coordinates)
	{
		_lavaTiles.Enqueue(coordinates);
		SetCell(coordinates, 0, new Vector2I(0, 0));
	}

	public void GenerateRandomLavaTile()
	{
		int x = GD.RandRange(0, _maxNotOutOfRangeCoordinateX);
		int y = GD.RandRange(0, _maxNotOutOfRangeCoordinateY);
		
		SetLavaTile(new Vector2I(x, y));
	}
	public void ClearLeastRecentLavaTile()
	{
		if (_lavaTiles.TryDequeue(out var coordinates))
			EraseCell(coordinates);
	}
	
	public void CoverWithLava() {
		for (int x = 0; x <= _maxNotOutOfRangeCoordinateX; ++x)
		for (int y = 0; y <= _maxNotOutOfRangeCoordinateY; ++y)
			SetLavaTile(new Vector2I(x, y));
	}	
	
	public void GetTiles(int value)
	{
		for (int i = 0; i < value; ++i) GenerateRandomLavaTile();
	}
	
	public override void _Ready()
	{
		//GD.Seed(12345);
		//GetTiles(5);
		//for (int i = 0; i < 5; ++i) ClearLeastRecentLavaTile();
	}
}
