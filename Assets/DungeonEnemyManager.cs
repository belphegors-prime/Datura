using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonEnemyManager : MonoBehaviour {
    public GameObject squadPrefab;
    public GameObject canvas;
    public Font definitelyNotDiablosFont;
    public int enemyFreq; //
    Room[] rooms;
	// Use this for initialization
	void Start()
    {
        rooms = GameObject.Find("Dungeon").GetComponentsInChildren<Room>();
        //if (enemyFreq > rooms.Length || enemyFreq <= 1) enemyFreq = 5;
        //SetEnemies();
    }

    void SetEnemies()
    {
        for(int i = enemyFreq; i < rooms.Length; i += enemyFreq)
        {
            //GameObject r = (GameObject)rooms[i];
            GameObject ds = (GameObject) Instantiate(squadPrefab, rooms[i].transform.position, Quaternion.identity);
            ds.AddComponent<DungeonSquadron>();
            ds.transform.SetParent(transform);
        }
    }
	// Update is called once per frame
	void Update () {
	    if(transform.childCount == 0)
        {
            Text t = canvas.AddComponent<Text>();
            t.text = "YOU DEFEATED";
            t.font = definitelyNotDiablosFont;
            t.fontSize = 32;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.red;
            GameManager.SetCurrentDungeonCompleted(true);
            SceneManager.LoadScene(GameManager.GetWorldScenePath());
        }
	}
}
