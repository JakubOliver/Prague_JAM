using Godot;
using System;

public partial class Floor : Node2D
{
	private TileMapLayer GetTileMapLayer() => GetNode<TileMapLayer>("TileMapLayer");
	
	public void ClearAllHoles()
	{
		TileMapLayer layer = GetTileMapLayer();
		
		foreach (var cellPosition in layer.GetUsedCells())
		{
			layer.SetCell(cellPosition, 1, new Vector2I(0,0));
		}
	}
	
	public void CastTileToAHole(Vector2I tilePosition)
	{
		TileMapLayer layer = GetTileMapLayer();
		layer.SetCell(tilePosition, 0, new Vector2I(0,0));
	}
	
	
}
