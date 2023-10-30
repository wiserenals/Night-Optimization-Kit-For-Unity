using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NightJob;
using NOK;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NavigationBuildJob : INightJob
{
    public NavigationWorld navigationWorld;
    public Vector3 position;
    public Vector3 targetPosition;
    public bool bake;
    public bool delayedObstacleBuild;
    public float obstacleBuildDelay;
    
    //generated
    public float tileRadius  { get; private set; }
    public Vector3 nodeSize { get; private set; }
    public NavigationTile[,] tiles { get; private set; }
    public float tileDiameter { get; private set; }
    public Stopwatch tileCreationTimer { get; private set; }
    public Stopwatch heatmapCreationTimer { get; private set; }
    public Stopwatch flowFieldCreationTimer { get; private set; }

    public bool JobCompleted { get; set; }

    private Vector2 worldSize;

    private Vector3 additionalPosition;

    public IEnumerator OnMainThread()
    {
        yield return null;
    }

    private bool[,] latestWalls;

    public async Task OnSecondaryThread()
    {
        tileCreationTimer = new Stopwatch();
        heatmapCreationTimer = new Stopwatch();
        flowFieldCreationTimer = new Stopwatch();
        
        tileRadius = navigationWorld.tileRadius;
        nodeSize = Vector3.one * (tileRadius * 2);
        
        tileDiameter = tileRadius * 2;

        bake = bake || latestWalls == null;

        worldSize = navigationWorld.navigationGridSize;
        int gridSizeX = Mathf.RoundToInt(worldSize.x / tileDiameter);
        int gridSizeY = Mathf.RoundToInt(worldSize.y / tileDiameter);

        latestWalls ??= navigationWorld.navInstance?.Wall;

        var navInstance = navigationWorld.navInstance 
            = new NavigationWorldInstance(
                gridSizeX, 
                gridSizeY, 
                latestWalls
                );
        
        #region CreateTileRegion

        tileCreationTimer.Start();
        
        Vector3 gridCenter = new Vector3(worldSize.x * 0.5f, 0, worldSize.y * 0.5f);
        additionalPosition = position - gridCenter;    
        
        var tempNodeArray = new NavigationTile[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 tilePosition = new Vector3(x * tileDiameter, 0, y * tileDiameter) + additionalPosition;
                NavigationTile tile = new NavigationTile(new Vector2Int(x, y), tilePosition);
                if (!bake && latestWalls != null && latestWalls[x, y]) tile.Weight = int.MaxValue;
                tempNodeArray[x, y] = tile;
            }
        }
        
        if (bake)
        {
            NavigationWorld navigationWorld1 = navigationWorld;
            NavigationBuildJob tmpThis = this;
            bool delayedObstacleBuild1 = delayedObstacleBuild;
            float obstacleBuildDelay1 = obstacleBuildDelay;
            var rad = tmpThis.tileRadius / 2;
            await NightJobManager.WaitForMainThread(async () =>
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    for (int y = 0; y < gridSizeY; y++)
                    {
                        var tile = tempNodeArray[x, y];
                        if (Physics.CheckSphere(tile.WorldPosition, rad, navigationWorld1.wallMask))
                        {
                            tile.Weight = int.MaxValue;
                            navInstance.Wall[x, y] = true;
                        }
                        navInstance.obstacleBuildTime++;
                        if(delayedObstacleBuild1) 
                            await NightJobManager.Yield(obstacleBuildDelay1);
                    }
                }
            });
        }

        navInstance.Tiles = tempNodeArray;
        navigationWorld.isAvailable = true;

        tileCreationTimer.Stop();

        #endregion
        
        tiles = navInstance.Tiles;
        
        #region HeatmapRegion

            heatmapCreationTimer.Start();
        
            var target = GetTileFromWorldPoint(targetPosition);
            target.Weight = 1;
            Queue<NavigationTile> visitQueue = new Queue<NavigationTile>();
            visitQueue.Enqueue(target);
            ProcessNeighbours(1, visitQueue, gridSizeX, gridSizeY);
            heatmapCreationTimer.Stop();

        #endregion

        #region FlowFieldRegion

            flowFieldCreationTimer.Start();

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    var tile = tiles[x, y];
                    Vector2Int pos = tile.GridPos;
                    Vector2 resultVector = Vector2.zero;

                    foreach (var neighbour in tile.GetNeighbours(gridSizeX, gridSizeY, tiles))
                    {
                        switch (neighbour)
                        {
                            case NavigationTile n:
                                resultVector -= ((Vector2)(n.GridPos - tile.GridPos)).normalized * n.Weight;
                                break;
                            case VoidTile v:
                                resultVector -= ((Vector2)(v.GridPos - tile.GridPos)).normalized * int.MaxValue;
                                break;
                        }
                        
                    }

                    tile.FlowFieldVector = resultVector.normalized;
                }
            }

            flowFieldCreationTimer.Stop();

        #endregion
        
        navigationWorld.tileCreationTime = tileCreationTimer.Elapsed;
        navigationWorld.heatmapCreationTime = heatmapCreationTimer.Elapsed;
        navigationWorld.flowFieldCreationTime = flowFieldCreationTimer.Elapsed;
    }

    private void ProcessNeighbours(int weight, Queue<NavigationTile> visitQueue, int gridSizeX, int gridSizeY)
    {
        while (true)
        {
            if (visitQueue.Count > 0)
            {
                NavigationTile currentTile = visitQueue.Dequeue();

                foreach (ITile neighbour in currentTile.GetNeighbours(gridSizeX, gridSizeY, tiles))
                {
                    switch (neighbour)
                    {
                        case NavigationTile n:
                            if (n.Weight == 0) visitQueue.Enqueue(n);
                            if (n.Weight < int.MaxValue)
                            {
                                n.Weight = ++weight;
                                navigationWorld.maxWeight = weight;
                            }
                            break;
                    }
                }
                
                continue;
            }

            break;
        }
    }
    /* Sonradan eklenecek wall escape özelliği
     Tile GUID'ler tutacak ve guid olup olmamasına göre next neighbour seçimi ve weight azaltılması işlemi gerçekleştirilecek
    private void StartPostProcessFlow(int weight, Queue<NavigationTile> visitQueue, int gridSizeX, int gridSizeY)
    {
        while (true)
        {
            if (visitQueue.Count > 0)
            {
                NavigationTile currentTile = visitQueue.Dequeue();

                foreach (ITile neighbour in currentTile.GetNeighbours(gridSizeX, gridSizeY, tiles))
                {
                    switch (neighbour)
                    {
                        case NavigationTile n:
                            if (n.Weight == 0) visitQueue.Enqueue(n);
                            if (n.Weight < int.MaxValue)
                            {
                                n.Weight = ++weight;
                                navigationWorld.maxWeight = weight;
                            }
                            break;
                    }
                }
                
                continue;
            }

            break;
        }
    }*/
    
    
    public Vector2 GetFlowFieldVector(Vector2 worldPoint)
    {
        Vector2 relativePosition = worldPoint - new Vector2(additionalPosition.x, additionalPosition.z);
        int x = Mathf.FloorToInt(relativePosition.x / tileDiameter);
        int y = Mathf.FloorToInt(relativePosition.y / tileDiameter);
        x = Mathf.Clamp(x, 0, tiles.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tiles.GetLength(1) - 1);
        return tiles[x, y].FlowFieldVector;
    }

    public NavigationTile GetTileFromWorldPoint(Vector2 worldPoint)
    {
        return GetTileFromWorldPoint(new Vector3(worldPoint.x, 0, worldPoint.y));
    }

    public NavigationTile GetTileFromWorldPoint(Vector3 worldPoint)
    {
        Vector3 relativePosition = worldPoint - additionalPosition;
        
        int x = Mathf.FloorToInt(relativePosition.x / tileDiameter);
        int y = Mathf.FloorToInt(relativePosition.z / tileDiameter);
        
        x = Mathf.Clamp(x, 0, tiles.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tiles.GetLength(1) - 1);
        
        return tiles[x, y];
    }
    

}