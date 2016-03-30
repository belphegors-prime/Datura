using UnityEngine;
using System.Collections;
[RequireComponent (typeof(NotificationManager))] // Component for sending and receiving notifications

public class GameManager : MonoBehaviour 
{

    public enum WorldSize { SMALL, LARGE};
    static WorldSize worldsize;

	// Internal reference to single active instance of object - for singleton behaviour
	private static GameManager instance = null;

	// Internal reference to notificationManager
	private static NotificationManager notifications = null;

    public static bool newWorld = true;
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

    /*pause should probably happen within GameManager
     public static PauseGame(){
        Time.timeScale = 0;
     * }*/
	void Awake()
	{
        //TEMPORARY: set world size to small for now
        worldsize = WorldSize.SMALL;

		// If duplicate of GameManager exists, destroy this object
		if(instance && instance.GetInstanceID() != GetInstanceID())
			DestroyImmediate(gameObject);

		else
		{
			instance = this; // Set current object as single GameManager
			DontDestroyOnLoad(gameObject); // When loading new scene, do not destroy object
		}
	}

    public static void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    public void SetWorldSize(WorldSize ws)
    {
        worldsize = ws;
    }

    public static WorldSize GetWorldSize()
    {
        return worldsize;
    }

    public static string GetWorldScenePath()
    {
        if (worldsize.Equals(WorldSize.LARGE))
        {
            return "Scenes/Worlds/Large/LargeWorld";
        }
        else
        {
            return "Scenes/Worlds/Small/SmallWorld";
        }
    }

}