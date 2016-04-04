using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WorldPortal : MonoBehaviour {
    PlayerController player;
    float activationDist = 5f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
    }

    void OnMouseDown()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < activationDist)
        {
            //Debug.Log("Player is within activation distance");
            //GameManager.WorldSize size = GameManager.GetWorldSize();
            //string scene = "";
            //if (size.Equals(GameManager.WorldSize.LARGE))
            //{
            //    scene = "Scenes/Worlds/Large/LargeWorld";
            //}
            //else if (size.Equals(GameManager.WorldSize.SMALL))
            //{
            //    scene = "Scenes/Worlds/Small/SmallWorld";
            //}

            SceneManager.LoadScene(GameManager.GetWorldScenePath());
        }
    }
}
