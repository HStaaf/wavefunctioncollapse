using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.WSA;

public class GroundLayer : Layers
{
    Vector3 padding;
    Vector3 downLeft;
    Vector3 upRight;
    Vector3 downRight 
    { 
        get { return new Vector3(upRight.x, downLeft.y); } 
    }
    Vector3 upLeft { 
        get { return new Vector3(downLeft.x, upRight.y); } 
    }
    HashSet<Vector3Int> placedTiles = new HashSet<Vector3Int>();
    Dictionary<Vector3Int, int> barredDirection = new Dictionary<Vector3Int, int>
    {
        { Vector3Int.down,  0},
        { Vector3Int.left,  1},
        { Vector3Int.up,    2},
        { Vector3Int.right, 3}
    };
    public RuleTile groundTile;
    public GrassLayer grassLayer;
    private JobHandle? expandLayerHandle;
    private bool isNonGroundLayerJobRunning;

    void Start()
    {
        padding  = new Vector3(3, 3);
        downLeft = tilemap.WorldToCell(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane))) - padding;
        upRight  = tilemap.WorldToCell(Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.nearClipPlane))) + padding ;        

        ExpandLayer(Vector3.zero);
        CustomEvents.OnUpdateGroundLayer += ExpandLayer;
    }

    void Update()
    {
        if (isNonGroundLayerJobRunning && expandLayerHandle.HasValue)
        {
            if (expandLayerHandle.Value.IsCompleted)
            {
                expandLayerHandle.Value.Complete(); // Ensure the job is fully completed
                isNonGroundLayerJobRunning = false;
                expandLayerHandle = null;
            }
        }
    }

    void ExpandLayer(Vector3 velocity)
    {
        HashSet<Vector3Int> result = PropagateGroundLayer(velocity);
        if (result.Count == 0) return;

        FillTiles(result, groundTile, placedTiles);

        if (!isNonGroundLayerJobRunning && velocity != Vector3.zero && rng.NextDouble() > 0.97)
        {
            Debug.Log("Scheduling new job.");
            isNonGroundLayerJobRunning = true;
            TileItem seed = PrepareSeed(result);
            expandLayerHandle = grassLayer.ExpandLayer(seed);
        }

        result.Clear();
    }

    TileItem PrepareSeed(HashSet<Vector3Int> newTiles)
    {
        Vector3Int seedPosition = newTiles.ElementAt(rng.Next(newTiles.Count)); ;
        int barredDirection = ObtainBarredDirections(seedPosition);

        return new TileItem(seedPosition, barredDirection, rng.Next(100,2000));
    }

    HashSet<Vector3Int> PropagateGroundLayer(Vector3 velocity)
    {
        downLeft += velocity * Time.deltaTime;
        upRight  += velocity * Time.deltaTime;

        Vector3Int downLeftInt  = ConvertToVector3Int(downLeft);
        Vector3Int upRightInt   = ConvertToVector3Int(upRight);
        Vector3Int upLeftInt    = ConvertToVector3Int(upLeft);
        Vector3Int downRightInt = ConvertToVector3Int(downRight);

        IEnumerable<Vector3Int> emptyTiles = Enumerable.Empty<Vector3Int>();

        if (velocity == Vector3.zero)
            emptyTiles = FindEmptyTiles(downLeftInt, upRightInt);

        else if (velocity.x > 0 && velocity.y > 0)
            emptyTiles = FindEmptyTiles(upLeftInt, upRightInt, downRightInt, upRightInt);
 
        else if (velocity.x < 0 && velocity.y > 0)
            emptyTiles = FindEmptyTiles(upLeftInt, upRightInt, downLeftInt, upLeftInt);
 
        else if (velocity.x > 0 && velocity.y < 0)
            emptyTiles = FindEmptyTiles(downLeftInt, downRightInt, downRightInt, upRightInt);

        else if (velocity.x < 0 && velocity.y < 0)
            emptyTiles = FindEmptyTiles(downLeftInt, downRightInt, downLeftInt, upLeftInt);


        return Enumerable.ToHashSet(emptyTiles);
    }

    int ObtainBarredDirections(Vector3Int seedPosition)
    {
        foreach (Vector3Int direction in neighborDirections.Take(4))
        {
            TileBase neighborTile = tilemap.GetTile(seedPosition + direction);
            if (neighborTile != null) continue;
            else return barredDirection[direction];
        }
        return default;
    }

    IEnumerable<Vector3Int> FindEmptyTiles(params Vector3Int[] corners)
    {
        if(corners.Length == 4)
        {
            var layer1 = FindTiles(corners[0], corners[1], placedTiles);
            var layer2 = FindTiles(corners[2], corners[3], placedTiles);

            return layer1.Concat(layer2).Distinct();
        }
        else  
        {
            var emptyTiles = FindTiles(corners[0], corners[1], placedTiles);
            return emptyTiles.Distinct();
        }
    }

    Vector3Int ConvertToVector3Int(Vector3 vector)
    {
        return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
    }
}
