using UnityEngine;
using System.Collections;

public class OldWorldManager : MonoBehaviour {
	public Tile tile;
	public int gridSize;
    public float waterThreshold;
    public int waterBuf; //number of tiles away from border where land tiles may begin to form

    float tileWidth, tileHeight, gridCenter;
    Tile[,] grid;
    Transform landParent, waterParent;
    MeshFilter[] meshfilters;
    CombineInstance[] combine;
    void SetSizes()
    {
        gridCenter = gridSize / 2;
        tileWidth = tile.GetComponent<Renderer>().bounds.size.x;
		tileHeight = tile.GetComponent<Renderer>().bounds.size.z;
        
	}
    /* for hex implementation
    void FormGrid()
    {
        Vector3 tileLoc = Vector3.zero;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Tile tileInst = (Tile) Instantiate(tile, tileLoc, Quaternion.identity);
                tileInst.name = "" + i + "," + j;
                tileInst.transform.SetParent(transform);
                tileLoc.x += hexWidth;
                tileInst.coordinates[0] = i;
                tileInst.coordinates[1] = j;
                grid[i, j] = tileInst;
            }
            tileLoc.x = 0f; //reset x position
            tileLoc.z -= hexHeight * .75f; //next line starts at 3/4 the height of a tile
            if (i % 2 == 0) tileLoc.x += hexWidth / 2f;
        }
    }
    */

    void FormGrid()
    {
        Vector3 tileLoc = Vector3.zero;
        int k = 0;

        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                Tile tileInst = (Tile)Instantiate(tile, tileLoc, Quaternion.identity);
                tileInst.name = "" + i + "," + j;
                tileInst.transform.SetParent(transform);
                tileLoc.x += tileWidth;
                tileInst.coordinates[0] = i;
                tileInst.coordinates[1] = j;
                grid[i, j] = tileInst;
                meshfilters[k] = tileInst.GetComponent<MeshFilter>();
                combine[k].mesh = meshfilters[k].sharedMesh;
                combine[k].transform = meshfilters[k].transform.localToWorldMatrix;
                k++;
            }
            tileLoc.x = 0f;
            tileLoc.z -= tileHeight;
        }
    }
    void SetNeighbors()
    {
        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0;  j < gridSize; j++)
            {
                Tile t = grid[i, j];
                //NW Corner
               if(i == 0 && j == 0)
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
                if (i == gridSize -1)
                {
                    t.neighbors[0] = grid[i - 1, j - 1];
                    t.neighbors[1] = grid[i - 1, j];
                    t.neighbors[2] = grid[i - 1, j + 1];
                    t.neighbors[3] = grid[i, j + 1];
                    t.neighbors[7] = grid[i, j - 1];
                    continue;
                }
                //Western Border
                if(j == 0)
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
    /*for hexagonal implementation
    void SetNeighbors()
    {
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                Tile t = grid[i, j];

                if (j > 0) t.neighbors[0] = grid[i, j - 1];
                else t.neighbors[0] = null;
                if (i > 0)
                {
                    t.neighbors[1] = grid[i - 1, j];
                    if (j < gridWidth - 1) t.neighbors[2] = grid[i - 1, j + 1];
                }
                else
                {
                    t.neighbors[1] = null;
                    t.neighbors[2] = null;
                }
                if (j < gridWidth - 1)
                {
                    t.neighbors[3] = grid[i, j + 1];
                    if(i < gridHeight - 1) t.neighbors[4] = grid[i + 1, j + 1];
                }
                else
                {
                    t.neighbors[3] = null;
                    t.neighbors[4] = null;
                }
                if (i < gridHeight - 1) t.neighbors[5] = grid[i + 1, j];
                else t.neighbors[5] = null;
            }
        }
    }
    */
    void CreateRoughCoasts()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Tile t = grid[i, j];
                //all border tiles should be water
                if (i < waterBuf || j < waterBuf || j >= gridSize - waterBuf || i >= gridSize - waterBuf)
                {
                    t.SetWaterOrLand(Tile.ISLAND.WATER);
                    t.transform.SetParent(waterParent);
                    
                }
                else
                {
                    float distFromCenter = Mathf.Sqrt(Mathf.Pow((i - gridCenter), 2) + Mathf.Pow((j - gridCenter), 2));
                    float pWater = distFromCenter / Mathf.Sqrt(2 * Mathf.Pow(gridCenter,2));
                    pWater += Random.Range(0, .2f);
                    if (pWater > waterThreshold)
                    {
                        t.SetWaterOrLand(Tile.ISLAND.WATER);
                        t.transform.SetParent(waterParent);
                    }
                    else
                    {
                        t.SetWaterOrLand(Tile.ISLAND.LAND);
                        t.transform.SetParent(landParent);
                    }
                }
            }
        }
    }

    //removes awkward tiny islands
    public void PolishCoasts()
    {
        for(int i = waterBuf; i < gridSize - waterBuf; i++)
        {
            for(int j = waterBuf; j < gridSize - waterBuf; j++)
            {
                Tile t = grid[i, j];
                if(t.island == Tile.ISLAND.LAND)
                {
                    int waterNeighbors = 0;
                    for(int n = 0; n < t.neighbors.Length; n++)
                    {
                        if (t.neighbors[n].island == Tile.ISLAND.WATER) waterNeighbors++;
                    }
                    if(waterNeighbors >= 6)
                    {
                        t.SetWaterOrLand(Tile.ISLAND.WATER);
                        t.transform.SetParent(waterParent);
                    }
                }
            }
        }
    }

    public void FormBiomes()
    {
        for(int i = 0; i < landParent.childCount; i++)
        {
            Tile t = landParent.GetChild(i).gameObject.GetComponent<Tile>();
            
            float distFromCenter = Mathf.Sqrt(Mathf.Pow((t.coordinates[0] - gridCenter), 2) + Mathf.Pow((t.coordinates[1] - gridCenter), 2));
            float biomeVal = distFromCenter / Mathf.Sqrt(2 * Mathf.Pow(gridCenter, 2));

            if (biomeVal <= .75 && biomeVal >= .5) t.SetBiome(Tile.BIOME.COAST);

            else if (biomeVal < .5 && biomeVal >= .15) t.SetBiome(Tile.BIOME.FOREST);

            else if (biomeVal < .15) t.SetBiome(Tile.BIOME.SNOW);
        }
    }
	// Use this for initialization
	void Start () 
    {
        landParent = transform.FindChild("Land");
        waterParent = transform.FindChild("Water");
        grid = new Tile[gridSize, gridSize];

        meshfilters = new MeshFilter[gridSize * gridSize];
        combine = new CombineInstance[meshfilters.Length];
		SetSizes();
        FormGrid();
        SetNeighbors();
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        //CreateRoughCoasts();

        //for(int i = 0; i < 4; i++) PolishCoasts();

        //FormBiomes();
	}

}
