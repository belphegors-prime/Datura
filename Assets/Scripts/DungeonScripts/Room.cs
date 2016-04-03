using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour {
    // Use this for initialization
    public int depth;
    public Room creator;

    public void SetUp(Room c, int d, string n)
    {
        gameObject.name = n;
        depth = d;
        creator = c;
    }
	
}
