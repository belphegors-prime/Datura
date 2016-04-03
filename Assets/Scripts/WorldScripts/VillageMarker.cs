using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VillageMarker : MonoBehaviour {
    PlayerController player;
    float activationDist = 3f;
    Tile tileLocation;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    public void SetTile(Tile t)
    {
        tileLocation = t;
    }
    public Tile GetTile()
    {
        return tileLocation;
    }
    void OnMouseDown()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < activationDist)
        {
            WorldManager.SaveWorld();
            SceneManager.LoadScene("Scenes/Village");
        }
    }
}
