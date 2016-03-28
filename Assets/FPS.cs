using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {
    GUIText blah;
    float t;
    // Use this for initialization
    void Start () {
        blah = this.gameObject.GetComponent<GUIText>();
	}
	
	// Update is called once per frame
	void Update () {
        float updateInterval = .5f;
        t += Time.deltaTime;
        if(t >= updateInterval)
        {
            blah.text = "FPS: " + 1 / Time.deltaTime;
            t = 0;
        }

	}
}
