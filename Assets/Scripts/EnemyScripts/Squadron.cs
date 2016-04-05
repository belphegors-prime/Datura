using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class Squadron : MonoBehaviour {
    public int enemyCount;
    public GameObject canvas;
    public Font definitelyNotDiablosFont;
    protected string path; //provided by subclasses
    Bounds spawnArea;
    
    void Awake()
    {
        spawnArea = transform.FindChild("SpawnArea").GetComponent<Collider>().bounds;

    }
	// Use this for initialization
	void Start ()
    {
        Initialize();
        CreateUnits();

	}
    void CreateUnits()
    {
        //randomly spawn enemies located in path
        Enemy[] units = Resources.LoadAll<Enemy>(path);
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = spawnArea.center;
            spawnPos.x = Random.Range(spawnArea.min.x, spawnArea.max.x);
            spawnPos.z = Random.Range(spawnArea.min.z, spawnArea.max.z);
            int rand = (int) Random.Range(0, units.Length - float.Epsilon);
            Enemy enemy = (Enemy) Instantiate(units[rand], spawnPos, Quaternion.identity);
            enemy.transform.SetParent(transform);

        }
    }
    // Update is called once per frame
    void Update()
    {
        //iterate over enemies, unparent if dead
        /*foreach(Transform unit in transform)
        {
            if (unit.GetComponent<Enemy>().dead)
            {
                unit.parent = null; 
            }
        }*/
        if(transform.GetComponentsInChildren<Enemy>().Length == 0)
        {
            Debug.Log("enemies cleared");
            Text t = canvas.AddComponent<Text>();
            t.text = "YOU DEFEATED";
            t.font = definitelyNotDiablosFont;
            t.fontSize = 32;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.red;

            SceneManager.LoadScene(GameManager.GetWorldScenePath());
        }
    }
    protected abstract void Initialize();

}
