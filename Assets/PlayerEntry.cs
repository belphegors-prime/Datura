using UnityEngine;
using System.Collections;

public class PlayerEntry : MonoBehaviour {
    PlayerController player;

	// Use this for initialization
	void Start () {
        player = PlayerController.player;
        player.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        player.transform.position = transform.position;
        player.gameObject.GetComponent<NavMeshAgent>().enabled = true;
	}
	
}
