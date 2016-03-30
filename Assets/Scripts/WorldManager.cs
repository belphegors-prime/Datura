using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class WorldManager : MonoBehaviour {
    public static int gridSize;
    public int waterBuf;
    public float waterThreshold;
    float gridCenter;
    Tile[,] grid;

    public VillageMarker vmPrefab;
    public DungeonMarker dmPrefab;
    public static int numVills = 5;
    public static int dungeonPerVill = 2;

    public static bool generated = false;

    GameObject landParent, waterParent, vmParent, dmParent;
    static WorldData saveData;

    //assign variables, create grid references, create persistent data
    void SetGridReferences()
    {
        gridCenter = gridSize / 2;

        grid = new Tile[gridSize, gridSize];
        int childIndex = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = transform.GetChild(childIndex).GetComponent<Tile>() as Tile;
                grid[i, j].coordinates[0] = i;
                grid[i, j].coordinates[1] = j;
                childIndex++;
            }
        }

        saveData = new WorldData();
    }
    void CreateLandWaterTransforms()
    {
        landParent = new GameObject();
        landParent.transform.SetParent(transform);
        landParent.name = "Land";
        waterParent = new GameObject();
        waterParent.transform.SetParent(transform);
        waterParent.name = "Water";
    }
    void SetNeighbors()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Tile t = grid[i, j];
                //NW Corner
                if (i == 0 && j == 0)
                {
                    t.neighbors[3] = grid[i, j + 1];
                    t.neighbors[4] = grid[i + 1, j + 1];
                    t.neighbors[5] = grid[i + 1, j];
                    continue;
                }
                //NE Corner
                if (i == 0 && j == gridSize - 1)
                {
                    t.neighbors[5] = grid[i + 1, j];
                    t.neighbors[6] = grid[i + 1, j - 1];
                    t.neighbors[7] = grid[i, j - 1];
                    continue;
                }
                //SE Corner
                if (i == gridSize - 1 && j == gridSize - 1)
                {
                    t.neighbors[0] = grid[i - 1, j - 1];
                    t.neighbors[1] = grid[i - 1, j];
                    t.neighbors[7] = grid[i, j - 1];
                    continue;
                }
                //SW Corner
                if (i == gridSize - 1 && j == 0)
                {
                    t.neighbors[1] = grid[i - 1, j];
                    t.neighbors[2] = grid[i - 1, j + 1];
                    t.neighbors[3] = grid[i, j + 1];
                    continue;
                }
                //Northern Border
                if (i == 0)
                {
                    t.neighbors[3] = grid[i, j + 1];
                    t.neighbors[4] = grid[i + 1, j + 1];
                    t.neighbors[5] = grid[i + 1, j];
                    t.neighbors[6] = grid[i + 1, j - 1];
                    t.neighbors[7] = grid[i, j - 1];
                    continue;
                }
                //Eastern Border
                if (j == gridSize - 1)
                {
                    t.neighbors[0] = grid[i - 1, j - 1];
                    t.neighbors[1] = grid[i - 1, j];
                    t.neighbors[5] = grid[i + 1, j];
                    t.neighbors[6] = grid[i + 1, j - 1];
                    t.neighbors[7] = grid[i, j - 1];
                    continue;
                }
                //Southern Border
                if (i == gridSize - 1)
                {
                    t.neighbors[0] = grid[i - 1, j - 1];
                    t.neighbors[1] = grid[i - 1, j];
                    t.neighbors[2] = grid[i - 1, j + 1];
                    t.neighbors[3] = grid[i, j + 1];
                    t.neighbors[7] = grid[i, j - 1];
                    continue;
                }
                //Western Border
                if (j == 0)
                {
                    t.neighbors[1] = grid[i - 1, j];
                    t.neighbors[2] = grid[i - 1, j + 1];
                    t.neighbors[3] = grid[i, j + 1];
                    t.neighbors[4] = grid[i + 1, j + 1];
                    t.neighbors[5] = grid[i + 1, j + 1];
                    continue;
                }
                //All Other Central Tiles
                t.neighbors[0] = grid[i - 1, j - 1];
                t.neighbors[1] = grid[i - 1, j];
                t.neighbors[2] = grid[i - 1, j + 1];
                t.neighbors[3] = grid[i, j + 1];
                t.neighbors[4] = grid[i + 1, j + 1];
                t.neighbors[5] = grid[i + 1, j];
                t.neighbors[6] = grid[i + 1, j - 1];
                t.neighbors[7] = grid[i, j - 1];
            }
        }
    }
    void CreateRoughCoasts()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Tile t = grid[i, j];
                //all border tiles should be water up to water buffer
                if (i < waterBuf || j < waterBuf || j >= gridSize - waterBuf || i >= gridSize - waterBuf)
                {
                    t.SetWaterOrLand(Tile.ISLAND.WATER);
                    t.transform.SetParent(waterParent.transform);
                }
                else
                {
                    float distFromCenter = Mathf.Sqrt(Mathf.Pow((i - gridCenter), 2) + Mathf.Pow((j - gridCenter), 2));
                    float pWater = distFromCenter / Mathf.Sqrt(2 * Mathf.Pow(gridCenter, 2));
                    pWater += UnityEngine.Random.Range(0, .2f);
                    if (pWater > waterThreshold)
                    {
                        t.SetWaterOrLand(Tile.ISLAND.WATER);
                        t.transform.SetParent(waterParent.transform);
                    }
                    else
                    {
                        t.SetWaterOrLand(Tile.ISLAND.LAND);
                        t.transform.SetParent(landParent.transform);
                    }
                }
            }
        }
    }
    public void PolishCoasts()
    {
        for (int i = waterBuf; i < gridSize - waterBuf; i++)
        {
            for (int j = waterBuf; j < gridSize - waterBuf; j++)
            {
                Tile t = grid[i, j];
                if (t.island == Tile.ISLAND.LAND)
                {
                    int waterNeighbors = 0;
                    for (int n = 0; n < t.neighbors.Length; n++)
                    {
                        if (t.neighbors[n])
                        {
                            if (t.neighbors[n].island == Tile.ISLAND.WATER) waterNeighbors++;
                        }
                    }
                    if (waterNeighbors >= 5)
                    {
                        t.SetWaterOrLand(Tile.ISLAND.WATER);
                        t.transform.SetParent(waterParent.transform);
                    }
                }
            }
        }
    }
    public void FormBiomes()
    {
        for (int i = 0; i < landParent.transform.childCount; i++)
        {
            Tile t = landParent.transform.GetChild(i).gameObject.GetComponent<Tile>();
            float distFromCenter = Mathf.Sqrt(Mathf.Pow((t.coordinates[0] - gridCenter), 2) + Mathf.Pow((t.coordinates[1] - gridCenter), 2));
            float biomeVal = distFromCenter / Mathf.Sqrt(2 * Mathf.Pow(gridCenter, 2));

            if (biomeVal <= .75 && biomeVal >= .5) t.SetBiome(Tile.BIOME.COAST);

            else if (biomeVal < .5 && biomeVal >= .15)
            {
                t.SetBiome(Tile.BIOME.FOREST);
            }

            else if (biomeVal < .15) t.SetBiome(Tile.BIOME.SNOW);
        }
    }
    void SetVillages()
    {
        //create parent object for village markers
        vmParent = new GameObject();
        vmParent.name = "VillageMarkers";
        //We would like villages to not be too close together
        //we define a minimum distance for cities to be the radius of the land continent
        //divided by the number of desired cities.
        //we can find the land radius using a simple heuristic -- the first and last child of the
        //land transform are the two furthest land tiles
        

        Vector3 p = landParent.transform.GetChild(0).position;
        Vector3 q = landParent.transform.GetChild(landParent.transform.childCount - 1).position;

        float landRadius = Vector3.Distance(p, q) / 2f;
        float minVillDist = landRadius / (numVills -1);

        ArrayList vmPositions = new ArrayList();
        for (int i = 0; i < numVills; i++)
        {
            //instantiate and select tile to place on
            VillageMarker vm = Instantiate(vmPrefab) as VillageMarker;
            Vector3 pos = Vector3.zero;
            int tileIndex = 0;
            while (true)
            {
                //randomly select land tile
                tileIndex = (int)UnityEngine.Random.Range(0, landParent.transform.childCount - 1);
                pos = landParent.transform.GetChild(tileIndex).position;

                //see if this tile satisfies minimum distance requirement                
                int j;
                for(j = 0; j < vmPositions.Count; j++)
                {
                    Vector3 v = (Vector3) vmPositions[j];
                    if (Vector3.Distance(v, pos) < minVillDist) break;
                }
                //if forloop completes, position is valid
                if (j == vmPositions.Count)
                {
                    vmPositions.Add(pos);
                    break;
                }
            }

            //adjust y value so that marker is ontop of ground 
            //pos.y += vm.GetComponent<Renderer>().bounds.size.y / 2f;
            vm.SetTile(landParent.transform.GetChild(tileIndex).GetComponent<Tile>());
            vm.transform.position = pos;
            vm.transform.SetParent(vmParent.transform);
        }
    }
    void SetDungeons()
    {
        dmParent = new GameObject();
        dmParent.name = "DungeonMarkers";
        //locate dungeons on tiles adjacent to village marker
        foreach(Transform child in vmParent.transform)
        {
            
            VillageMarker vm = child.GetComponent<VillageMarker>();
            ArrayList takenTiles = new ArrayList();
            for(int i = 0; i < dungeonPerVill; i++)
            {
                Tile t = vm.GetTile();
                Vector3 pos = new Vector3();
                int neighborIndex;
                //find available adjacent tile
                while (true)
                {
                    neighborIndex = (int)UnityEngine.Random.Range(0, t.neighbors.Length - float.Epsilon);
                    if (!takenTiles.Contains(neighborIndex))
                    {
                        takenTiles.Add(neighborIndex);
                        pos = t.neighbors[neighborIndex].transform.position;
                        break;
                    }
                }
                DungeonMarker dm = Instantiate(dmPrefab, pos, Quaternion.identity) as DungeonMarker;
                dm.SetTile(t.neighbors[neighborIndex]);
                dm.transform.SetParent(dmParent.transform);
                dm.transform.rotation = dmPrefab.transform.rotation;
            }
        }
    }
    void CreatePersistentData()
    {
        //store tile info
        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                Tile t = grid[i, j];
                saveData.landOrWater[i, j] = (int)t.island;
                saveData.biomes[i, j] = (int)t.biome;
            }
        }

       for(int i = 0; i < vmParent.transform.childCount; i++)
        {
            VillageMarker vm = vmParent.transform.GetChild(i).GetComponent<VillageMarker>();
            saveData.vmTiles[i] = vm.GetTile().coordinates;
        }
       for(int i = 0; i < dmParent.transform.childCount; i++)
        {
            DungeonMarker dm = dmParent.transform.GetChild(i).GetComponent<DungeonMarker>();
            saveData.dmTiles[i] = dm.GetTile().coordinates;
        }
        saveData.lastPlayerLocation[0] = PlayerController.player.transform.position.x;
        saveData.lastPlayerLocation[1] = PlayerController.player.transform.position.y;
        saveData.lastPlayerLocation[2] = PlayerController.player.transform.position.z;
    }
    public static void SaveWorld()
    {

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/world_data.dat", FileMode.Create);

        bf.Serialize(file, saveData);
        file.Close();
    }

    public static void LoadWorld()
    {
        if(File.Exists(Application.persistentDataPath + "/world_data.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/world_data.dat", FileMode.Open);

            saveData = (WorldData) bf.Deserialize(file);
            file.Close();

        }
    }
    void Start()
    {
        if (GameManager.newWorld)
        { 
            SetGridReferences();
            SetNeighbors();
            CreateLandWaterTransforms();
            CreateRoughCoasts();
            for (int i = 0; i < 4; i++) PolishCoasts();
            FormBiomes();
            SetVillages();
            SetDungeons();
            CreatePersistentData();
            GameManager.newWorld = false;
        }
        else
        {
            LoadWorld();
        }
    }


}

[Serializable]
class WorldData
{
    //stores land/water info, biome info, location info for village and dungeon markers
    public int[,] landOrWater = new int[WorldManager.gridSize, WorldManager.gridSize];
    public int[,] biomes = new int[WorldManager.gridSize, WorldManager.gridSize];
    public int[][] vmTiles = new int[WorldManager.numVills][];
    public int[][] dmTiles = new int[WorldManager.numVills * WorldManager.dungeonPerVill][];

    public float[] lastPlayerLocation = new float[3];
}