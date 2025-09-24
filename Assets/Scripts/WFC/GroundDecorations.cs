using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundDecorations : MonoBehaviour
{
    public Tilemap tilemap;

    public RuleTile GroundCracks;

    public void ExpandLayer(HashSet<Vector3Int> placedTiles)
    {
        foreach (var position in placedTiles)
        {
            if (Random.value < 0.01f)
            {
                tilemap.SetTile(position, GroundCracks);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
