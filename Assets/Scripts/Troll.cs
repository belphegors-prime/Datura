using UnityEngine;
using System.Collections;

public class Troll : Enemy 
{
	// Use this for initialization
	protected override void Initialize()
	{
		// Character attributes
		characterName = "Troll";
		maxHealth = 300.0f;
		currentHealth = 300.0f;
		attackDelay = 15.0f;
		damage = 5.0f;
		patrolRange = 10.0f;
		chaseRange = 12.0f;
		attackRange = 3.0f;
		
		// Set player reference and start AI
		player = PlayerController.player;
		ChangeState(activeState);
	}
}