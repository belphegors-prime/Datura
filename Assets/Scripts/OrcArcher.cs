using UnityEngine;
using System.Collections;

public class OrcArcher : Enemy 
{
	// Use this for initialization
	protected override void Initialize()
	{
		// Character attributes
		characterName = "Orc Archer";
		maxHealth = 100.0f;
		currentHealth = 100.0f;
		attackDelay = 2.0f;
		damage = 5.0f;
		patrolRange = 10.0f;
		chaseRange = 12.0f;
		attackRange = 10.0f;
		
		// Set player reference and start AI
		player = PlayerController.player;
		ChangeState(activeState);
	}
}