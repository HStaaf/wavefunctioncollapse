using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class NonGroundLayer : Layers
{
    public RuleTile grassTile;
    public System.Random random = new System.Random();

    public JobHandle ExpandLayer(TileItem seed)
    {
        // Step 1: Calculate Tile Positions
        var calculateTilePositionsJob = new CalculateTilePositionsJob
        {
            newTilesSet = new NativeHashSet<Vector3Int>(seed.numberOfTiles, Allocator.TempJob),
            tileSet = new NativeHashSet<Vector3Int>(seed.numberOfTiles, Allocator.TempJob),
            gapsDictionary = new NativeHashMap<Vector3Int, int>(0, Allocator.TempJob),
            neighborDirections = new NativeHashSet<Vector3Int>(8, Allocator.TempJob),

            barredDirection = seed.barredDirection,
            seedPosition = seed.tilePosition,
            numberOfTiles = seed.numberOfTiles,
            randomSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue),

            noMoreGaps = false,
            //hasGapsMoreThanFive = true
        };

        //Filling the hashsets of the job
        foreach(var direction in neighborDirections) calculateTilePositionsJob.neighborDirections.Add(direction);
        calculateTilePositionsJob.newTilesSet.Add(seed.tilePosition);
        calculateTilePositionsJob.tileSet.Add(seed.tilePosition);

        JobHandle calculateTilePositionsHandle = calculateTilePositionsJob.Schedule();

        calculateTilePositionsHandle.Complete();
        ProcessPositionsSet(calculateTilePositionsJob.newTilesSet);

        //Disposing the hashsets of the job
        calculateTilePositionsJob.newTilesSet.Dispose();
        calculateTilePositionsJob.tileSet.Dispose();
        calculateTilePositionsJob.gapsDictionary.Dispose();
        calculateTilePositionsJob.neighborDirections.Dispose();

        return calculateTilePositionsHandle;
    }

    private void ProcessPositionsSet(NativeHashSet<Vector3Int> positionsSet)
    {
        foreach (var position in positionsSet)
        {
            tilemap.SetTile(position, grassTile);
        }

    }

    [BurstCompile]
    struct CalculateTilePositionsJob : IJob
    {
        public NativeHashSet<Vector3Int> newTilesSet;
        public NativeHashSet<Vector3Int> tileSet;
        public NativeHashSet<Vector3Int> neighborDirections;
        public NativeHashMap<Vector3Int, int> gapsDictionary;

        public Vector3Int seedPosition;
        public int barredDirection;
        public int numberOfTiles;
        public uint randomSeed;

        public bool noMoreGaps;
        public bool hasGapsMoreThanFive;

        public void Execute()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(randomSeed);

            int tileCounter = 0;

            while (tileCounter < numberOfTiles && tileSet.Count() > 0)
            {
                int randomIndex = rng.NextInt(0, tileSet.Count());
                Vector3Int tilePosition = GetElementAt(tileSet, randomIndex);

                foreach (var direction in neighborDirections)
                {
                    Vector3Int newTilePosition = tilePosition + direction;
                    switch (barredDirection)
                    {
                        case  0:
                            if (newTilePosition.y > seedPosition.y) break;
                            if (newTilesSet.Add(newTilePosition))
                            {
                                tileSet.Add(newTilePosition);
                                tileCounter++;
                            }
                            break;
                        case  1:
                            if (newTilePosition.x > seedPosition.x) break;
                            if (newTilesSet.Add(newTilePosition))
                            {
                                tileSet.Add(newTilePosition);
                                tileCounter++;
                            }
                            break;
                        case  2:
                            if (newTilePosition.y < seedPosition.y) break;
                            if (newTilesSet.Add(newTilePosition))
                            {
                                tileSet.Add(newTilePosition);
                                tileCounter++;
                            }
                            break;
                        case  3:
                            if (newTilePosition.x < seedPosition.x) break;
                            if (newTilesSet.Add(newTilePosition))
                            {
                                tileSet.Add(newTilePosition);
                                tileCounter++;
                            }
                            break;
                        default:
                            break;
                    }
                }

                tileSet.Remove(tilePosition); 
            }

            while (!noMoreGaps)
            {
                gapsDictionary.Clear();

                foreach (var position in newTilesSet)
                {
                    foreach (var direction in neighborDirections)
                    {
                        Vector3Int checkTilePosition = position + direction;

                        if (!newTilesSet.Contains(checkTilePosition))
                        {
                            if (gapsDictionary.ContainsKey(checkTilePosition))
                            {
                                gapsDictionary[checkTilePosition] += 1;
                            }
                            else
                            {
                                // Key does not exist, add new key-value pair
                                gapsDictionary.Add(checkTilePosition, 1);
                            }
                        }
                    }
                }
                hasGapsMoreThanFive = false;

                foreach (var gap in gapsDictionary)
                {
                    if (gap.Value > 5)
                    {
                        newTilesSet.Add(gap.Key);
                        hasGapsMoreThanFive = true;
                    }
                }

                if (!hasGapsMoreThanFive) noMoreGaps = true;
            }
        }

        Vector3Int GetElementAt(NativeHashSet<Vector3Int> set, int index)
        {
            int currentIndex = 0;
            foreach (var element in set)
            {
                if (currentIndex == index)
                {
                    return element;
                }
                currentIndex++;
            }
            return default;
        }
    }



}
