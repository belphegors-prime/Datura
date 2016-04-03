using UnityEngine;
using System.Collections;

public class Beetle : Enemy 
{
	// Use this for initialization
	protected override void Initialize()
	{
		// Character attributes
		characterName = "Beetle";
		maxHealth = 150.0f;
		currentHealth = 150.0f;
		attackDelay = 2.0f;
		damage = 5.0f;
		patrolRange = 10.0f;
		chaseRange = 6.0f;
		attackRange = 3.0f;
		morale = 100.0f;
		minimumFightingMorale = 5.0f;
		//willFlee = true;
		
		// Set player reference and start AI
		player = PlayerController.player;
		ChangeState(activeState);
	}
}
