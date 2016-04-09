using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DungeonMarker : MonoBehaviour {
    public PlayerController player;
 
    Tile tileLocation;
    int id = -1;
    float activationDist = 3f;
    bool completed = false;

    public int GetID()
    {
        return id;
    }
    public void SetID(int i)
    {
        if (i < 0) return;
        else id = i;
    }
    public Tile GetTile()
    {
        return tileLocation;
    }

    public void SetTile(Tile t)
    {
        tileLocation = t;
    }
    
    public bool IsCompleted()
    {
        return completed;
    } 

    public void SetCompleted(bool c)
    {
        completed = c;
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnMouseDown()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= activationDist)
        {
            //track dungeon marker by its coordinates
            if(tileLocation.coordinates == null) Debug.Log("null tile location");
            GameManager.SetCurrentDungeon(tileLocation.coordinates);
            WorldManager.SaveWorld();
            SceneManager.LoadScene("Scenes/Dungeon");
        }
    }


}
