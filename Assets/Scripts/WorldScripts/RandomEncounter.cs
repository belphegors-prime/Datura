using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RandomEncounter : MonoBehaviour {
    public float encounterProb;
    PlayerController player;
    void Start()
    {
        player = PlayerController.player;   
    }
	// Update is called once per frame
	void Update ()
    {
        Tile.BIOME b;
	    if(Random.value < encounterProb)
        {
            WorldManager.SaveWorld();
            b = GetBiome();
            switch (b)
            {
                /*case Tile.BIOME.COAST:
                    WorldManager.SaveWorld();
                    SceneManager.LoadScene("EncounterScenes/CoastEncounter");
                    break;*/

                case Tile.BIOME.SNOW:
                    
                    SceneManager.LoadScene("Scenes/EncounterScenes/SnowEncounter");
                    break;

                case Tile.BIOME.FOREST:
                    SceneManager.LoadScene("Scenes/EncounterScenes/ForestEncounter2");
                    break;

            }
        }
	}

    Tile.BIOME GetBiome()
    {
        RaycastHit rch;
        if (Physics.Raycast(player.transform.position, Vector3.down, out rch))
        {
            Tile t = rch.collider.GetComponent<Tile>();
            return t.biome;
        }
        else return Tile.BIOME.OCEAN;
    }
}
