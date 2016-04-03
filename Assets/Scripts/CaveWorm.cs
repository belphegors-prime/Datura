using UnityEngine;
using System.Collections;

public class CaveWorm : Enemy 
{
	// Use this for initialization
	protected override void Initialize()
	{
		// Character attributes
		characterName = "Cave Worm";
		maxHealth = 125.0f;
		currentHealth = 125.0f;
		attackDelay = 2.0f;
		damage = 5.0f;
		patrolRange = 10.0f;
		chaseRange = 12.0f;
		attackRange = 3.0f;
		
		// Set player reference and start AI
		player = PlayerController.player;
		ChangeState(activeState);
	}
}