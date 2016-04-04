﻿using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    public Tile[] neighbors;
    public enum ISLAND { LAND, WATER };
    public enum BIOME {  OCEAN, COAST, FOREST, DESERT, SNOW};
    Material WaterMat, CoastMat, ForestMat, SnowMat, DesertMat;

    public ISLAND island;
    public BIOME biome;
    public int[] coordinates;
	// Use this for initialization
	void Awake ()
    {
        neighbors = new Tile[8];
        coordinates = new int[2];

        WaterMat = Resources.Load("Materials/Water") as Material;
        CoastMat = Resources.Load("Materials/Coast") as Material;
        ForestMat = Resources.Load("Materials/Forest") as Material;
        SnowMat = Resources.Load("Materials/Snow") as Material;

	}

    public bool isWater()
    {
        if (island == ISLAND.WATER) return true;
        else return false;
    }
    public void SetWaterOrLand(ISLAND i)
    {
        island = i;
        if( i == ISLAND.WATER)
        {
            gameObject.GetComponent<Renderer>().material = WaterMat;
            gameObject.AddComponent<NavMeshObstacle>();
            gameObject.GetComponent<NavMeshObstacle>().carving = true;
            gameObject.GetComponent<NavMeshObstacle>().height = 5;
        }
    }
    public void SetBiome(BIOME b)
    {
        biome = b;
        if (b == BIOME.COAST) gameObject.GetComponent<Renderer>().material = CoastMat;
        else if (b == BIOME.FOREST) gameObject.GetComponent<Renderer>().material = ForestMat;
        else if (b == BIOME.DESERT) gameObject.GetComponent<Renderer>().material = DesertMat;
        else if (b == BIOME.SNOW) gameObject.GetComponent<Renderer>().material = SnowMat;
        else if (b == BIOME.OCEAN) gameObject.GetComponent<Renderer>().material = WaterMat;
    }
}