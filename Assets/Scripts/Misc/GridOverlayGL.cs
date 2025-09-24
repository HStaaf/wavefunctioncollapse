using UnityEngine;
using UnityEngine.Tilemaps;

public class GridOverlayGL : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap component
    public Material gridMaterial; // Reference to the custom material
    public Color gridColor = Color.white; // Color of the grid lines
    private int gridWidth = 20; // Width of the grid
    private int gridHeight = 20; // Height of the grid

    void OnPostRender()
    {
        if (tilemap == null || gridMaterial == null)
        {
            Debug.LogWarning("Tilemap or GridMaterial is not assigned.");
            return;
        }

        // Set the material for drawing
        gridMaterial.SetPass(0);
        GL.PushMatrix();
        GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
        GL.modelview = Camera.main.worldToCameraMatrix;
        GL.Begin(GL.LINES);
        GL.Color(gridColor);

        Vector3Int origin = tilemap.origin;
        Vector3 cellSize = tilemap.cellSize;

        // Draw vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = tilemap.CellToWorld(new Vector3Int(origin.x + x, origin.y, 0));
            Vector3 end = tilemap.CellToWorld(new Vector3Int(origin.x + x, origin.y + gridHeight, 0));
            GL.Vertex(start);
            GL.Vertex(end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = tilemap.CellToWorld(new Vector3Int(origin.x, origin.y + y, 0));
            Vector3 end = tilemap.CellToWorld(new Vector3Int(origin.x + gridWidth, origin.y + y, 0));
            GL.Vertex(start);
            GL.Vertex(end);
        }

        GL.End();
        GL.PopMatrix();
    }

    void OnDrawGizmos()
    {
        if (tilemap == null)
            return;

        Vector3Int origin = tilemap.origin;
        Vector3 cellSize = tilemap.cellSize;

        Gizmos.color = gridColor;

        // Draw vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = tilemap.CellToWorld(new Vector3Int(origin.x + x, origin.y, 0));
            Vector3 end = tilemap.CellToWorld(new Vector3Int(origin.x + x, origin.y + gridHeight, 0));
            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = tilemap.CellToWorld(new Vector3Int(origin.x, origin.y + y, 0));
            Vector3 end = tilemap.CellToWorld(new Vector3Int(origin.x + gridWidth, origin.y + y, 0));
            Gizmos.DrawLine(start, end);
        }
    }
}
