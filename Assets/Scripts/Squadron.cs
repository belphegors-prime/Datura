using UnityEngine;
using System.Collections;

public class Squadron : MonoBehaviour {
   
    string path = "ForestMonsters";
    Bounds spawnArea;

    void Awake()
    {
        spawnArea = transform.FindChild("SpawnArea").GetComponent<Collider>().bounds;

    }
	// Use this for initialization
	void Start () {
        GameObject[] units = (GameObject[]) Resources.LoadAll(path);
        for(int i = 0; i < units.Length; i++)
        {
            Vector3 spawnPos = spawnArea.center;
            spawnPos.x = Random.Range(spawnArea.min.x, spawnArea.max.x);
            spawnPos.z = Random.Range(spawnArea.min.z, spawnArea.max.z); 
            GameObject enemy = (GameObject) Instantiate(units[i], spawnPos, Quaternion.identity);
            
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
