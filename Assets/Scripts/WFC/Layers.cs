using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Layers : MonoBehaviour
{

    protected Vector3Int[] neighborDirections = new Vector3Int[] {
    Vector3Int.up, // North
    Vector3Int.down, // South
    Vector3Int.left, // West
    Vector3Int.right, // East
    Vector3Int.up + Vector3Int.right, // Northeast
    Vector3Int.up + Vector3Int.left, // Northwest
    Vector3Int.down + Vector3Int.right, // Southeast
    Vector3Int.down + Vector3Int.left // Southwest
};
    public System.Random rng = new System.Random();
    public Tilemap tilemap;

    protected void FillTiles<T>(HashSet<Vector3Int> newTiles, RuleTile tile, T placedTiles) where T : ICollection<Vector3Int>
    {
        foreach (var position in newTiles)
        {
            if (tilemap.GetTile(position) == null)
            {
                tilemap.SetTile(position, tile);
                placedTiles.Add(position);
            }
        }
    }

    protected HashSet<Vector3Int> FindTiles<T>(Vector3Int start, Vector3Int end, T placedTiles) where T : IEnumerable<Vector3Int>
    {
        var positions = Enumerable.Range(start.x, Mathf.Abs(end.x - start.x) + 1)
            .SelectMany(x => Enumerable.Range(start.y, Mathf.Abs(end.y - start.y) + 1),
                (x, y) => new Vector3Int(x, y, start.z));

        return positions.Where(pos => !placedTiles.Contains(pos)).ToHashSet();
    }

}
