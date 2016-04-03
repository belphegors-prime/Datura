using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour 
{	
	// Character attributes
	protected string characterName;
	public float maxHealth;
	public float currentHealth;
	protected float damage;
	protected float attackDelay;

	// Audio source and navigation mesh agent components
	protected AudioSource audioSource;
	protected NavMeshAgent navAgent;	
	protected Animation characterAnimation;

	// Public audio clip and animations
	public AudioClip impactSound;

	protected AnimationEvent attackEvent = new AnimationEvent(); // Attack animation event

	// Function is called when scene starts
	void Awake()
	{
		audioSource = GetComponent<AudioSource>(); // Get component thats plays Audio
		navAgent = GetComponent<NavMeshAgent>(); // Get component that moves character
		characterAnimation = GetComponent<Animation>(); // Get component that plays animations
	}

	public string getName()
	{
		return this.characterName;
	}

	public float getMaxHealth()
	{
		return this.maxHealth;
	}

	public float getCurrentHealth()
	{
		return this.currentHealth;
	}
}