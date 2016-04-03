using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DungeonMarker : MonoBehaviour {
    public PlayerController player;

    Tile tileLocation;
    float activationDist = 3f;

    public Tile GetTile()
    {
        return tileLocation;
    }
    public void SetTile(Tile t)
    {
        tileLocation = t;
    } 
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnMouseDown()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= activationDist)
        {
            WorldManager.SaveWorld();
            SceneManager.LoadScene("Scenes/Dungeon");
        }
    }
}
