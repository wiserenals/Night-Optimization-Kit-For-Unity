using System.Collections.Generic;
using UnityEngine;

public interface ITile
{
    public Vector2Int GridPos { get; }
}

public class VoidTile : ITile
{
    public VoidTile(Vector2Int gridPos)
    {
        GridPos = gridPos;
    }

    public Vector2Int GridPos { get; }
}

public class NavigationTile : ITile
{
    public Vector2Int GridPos { get; }
    public int Weight { get; set; }
    public Vector3 WorldPosition { get; }
    public Vector2 FlowFieldVector { get; set; }

    public NavigationTile(Vector2Int gridPos, Vector3 worldPosition)
    {
        Weight = 0;
        GridPos = gridPos;
        WorldPosition = worldPosition;
        FlowFieldVector = Vector2.zero;
    }
    
    private bool IsValid(int x, int y, float gridSizeX, float gridSizeY)
    {
        return x >= 0 && y >= 0 && x < gridSizeX && y < gridSizeY;
    }

    public List<ITile> GetNeighbours(int gridSizeX, int gridSizeY, NavigationTile[,] grid)
    {
        List<ITile> res = new List<ITile>();
        int[] dx = { 0, 1, 0, -1, 1, 1, -1, -1 };
        int[] dy = { -1, 0, 1, 0, 1, -1, 1, -1 };

        for (int i = 0; i < 8; i++)
        {
            int newX = GridPos.x + dx[i];
            int newY = GridPos.y + dy[i];
            if (IsValid(newX, newY, gridSizeX, gridSizeY)) res.Add(grid[newX, newY]);
            else
            {
                res.Add(new VoidTile(new Vector2Int(newX, newY)));
            }
        }

        return res;
    }
}
