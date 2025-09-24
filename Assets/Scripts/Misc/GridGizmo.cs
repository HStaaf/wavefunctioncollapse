using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridGizmo : MonoBehaviour
{
    public Tilemap tilemap;
    public Color gridColor = Color.white;

    void OnDrawGizmos()
    {
        if (tilemap == null) return;

        Vector3Int origin = tilemap.origin;
        Vector3 cellSize = tilemap.cellSize;
        int width = tilemap.size.x;
        int height = tilemap.size.y;

        Gizmos.color = gridColor;

        // Draw vertical lines
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = tilemap.CellToWorld(new Vector3Int(origin.x + x, origin.y, 0));
            Vector3 end = tilemap.CellToWorld(new Vector3Int(origin.x + x, origin.y + height, 0));
            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = tilemap.CellToWorld(new Vector3Int(origin.x, origin.y + y, 0));
            Vector3 end = tilemap.CellToWorld(new Vector3Int(origin.x + width, origin.y + y, 0));
            Gizmos.DrawLine(start, end);
        }
    }
}
