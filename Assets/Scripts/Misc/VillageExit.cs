using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VillageExit : MonoBehaviour {

    void OnTriggerEnter(Collider c)
    {
        if (c.tag.Equals("Player"))
        {
            SceneManager.LoadScene(GameManager.GetWorldScenePath());
        }
    }
}
