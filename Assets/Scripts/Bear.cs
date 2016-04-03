﻿using UnityEngine;
using System.Collections;

public class Bear : Enemy 
{
	// Use this for initialization
	protected override void Initialize()
	{
		// Character attributes
		characterName = "Bear";
		maxHealth = 300.0f;
		currentHealth = 300.0f;
		attackDelay = 2.0f;
		damage = 5.0f;
		patrolRange = 10.0f;
		chaseRange = 12.0f;
		attackRange = 3.0f;
		morale = 100.0f;
		minimumFightingMorale = 50.0f;
		bleedWhenDamaged = true;
		
		// Set player reference and start AI
		player = PlayerController.player;
		ChangeState(activeState);
	}
}