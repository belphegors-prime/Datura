using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float height = 12;
	public float radius = 10;
	public float angle = 0;
	public float rotationalSpeed = 5;

	void Start()
	{
		target = PlayerController.player.transform;
	}

	// Update is called once per frame
	void Update() 
	{
		if(!target) target = PlayerController.player.transform;
		// Set default camera position coordinates
		float cameraX = target.position.x + radius * Mathf.Cos(angle);
		float cameraY = target.position.y + height;
		float cameraZ = target.position.z + radius * Mathf.Sin(angle);
		transform.position = new Vector3(cameraX, cameraY, cameraZ);
		
		// Modify camera coordinates if A or D key are pressed
		if(Input.GetKey(KeyCode.A)) angle = angle - rotationalSpeed * Time.deltaTime;
		else if(Input.GetKey(KeyCode.D)) angle = angle + rotationalSpeed * Time.deltaTime;
		
		transform.LookAt(target); // Set camera position
	}
}
