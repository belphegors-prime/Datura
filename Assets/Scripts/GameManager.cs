using UnityEngine;
using System.Collections;
[RequireComponent (typeof(NotificationManager))] // Component for sending and receiving notifications

public class GameManager : MonoBehaviour 
{
	// Internal reference to single active instance of object - for singleton behaviour
	private static GameManager instance = null;

	// Internal reference to notificationManager
	private static NotificationManager notifications = null;

	public static GameManager Instance
	{
		get
		{
			if(instance == null) 
				instance = new GameObject("GameManager").AddComponent<GameManager>();

			return instance;
		}
	}

	public static NotificationManager Notifications
	{
		get
		{
			if(notifications == null) 
				notifications = instance.GetComponent<NotificationManager>();

			return notifications;
		}
	}

	void Awake()
	{
		// If duplicate of GameManager exists, destroy this object
		if(instance && instance.GetInstanceID() != GetInstanceID())
			DestroyImmediate(gameObject);

		else
		{
			instance = this; // Set current object as single GameManager
			DontDestroyOnLoad(gameObject); // When loading new scene, do not destroy object
		}
	}
}