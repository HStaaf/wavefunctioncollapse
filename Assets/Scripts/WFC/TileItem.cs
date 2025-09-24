using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct TileItem
{
    public readonly Vector3Int tilePosition;
    public readonly int barredDirection;
    public readonly int numberOfTiles;

    public TileItem(Vector3Int sP, int bd, int t)
    {
        tilePosition    = sP;
        barredDirection = bd;
        numberOfTiles   = t;
    }

    public TileItem(Vector3Int newTilePosition, TileItem tileItemToCopy)
    {
        tilePosition = newTilePosition;
        numberOfTiles = tileItemToCopy.numberOfTiles;
        barredDirection = tileItemToCopy.barredDirection;
    }
}

