using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour 
{
	// Internal reference to all listeners for notifications
	public Dictionary<string, List<Component>> listenerDictionary = new Dictionary<string, List<Component>>();

	public void AddListener(Component listener, string notificationName)
	{
		// If dictionary does not have notification, add notification type
		if(!listenerDictionary.ContainsKey(notificationName))
		   listenerDictionary.Add(notificationName, new List<Component>());

		// Add object to listener dictionary
		listenerDictionary[notificationName].Add(listener);
	}

	public void PostNotification(Component sender, string notificationName)
	{
		// If dictionary does not have notification, exit
		if(!listenerDictionary.ContainsKey(notificationName)) return;
		
		// Post notification to all listeners
		foreach(Component listener in listenerDictionary[notificationName])
		{
			listener.SendMessage(notificationName, sender, 
				SendMessageOptions.DontRequireReceiver);
		}
	}

	public void RemoveListener(Component sender, string notificationName)
	{
		// If dictionary does not have notification, exit
		if(!listenerDictionary.ContainsKey(notificationName)) return;
		
		// Cycle through all listeners of a notification, remove sender if found
		for(int i = listenerDictionary[notificationName].Count - 1; i >= 0; i--)
		{
			if(listenerDictionary[notificationName][i].GetInstanceID() == sender.GetInstanceID())
				listenerDictionary[notificationName].RemoveAt(i);
		}
	}

	public void RemoveRedundancies()
	{
		// Create new dictionary
		Dictionary<string, List<Component>> newListenerDictionary = new Dictionary<string, List<Component>>();

		foreach(KeyValuePair<string, List<Component>> item in listenerDictionary)
		{
			for(int i = item.Value.Count - 1; i >= 0; i--)
			{
				// If listener is null, remove it from dictionary
				if(item.Value[i] == null) item.Value.RemoveAt(i);

				// If items remain in list for this notification, then add notification to new dicitionary
				newListenerDictionary.Add(item.Key, item.Value);
			}
		}

		listenerDictionary = newListenerDictionary; // Replace listener dictionary with new one
	}

	public void ClearListeners()
	{
		listenerDictionary.Clear();
	}

	// When level is loaded, remove all listeners equal to null in listener dictionary
	void OnLevelWasLoaded()
	{
		RemoveRedundancies();
	}
}