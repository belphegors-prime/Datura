using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DungeonMarker : MonoBehaviour {
    public PlayerController player;

    float activationDist = 3f;
    void OnMouseDown()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= activationDist)
        {
            SceneManager.LoadScene("Scenes/Dungeon/Dungeon");
        }
    }
}
