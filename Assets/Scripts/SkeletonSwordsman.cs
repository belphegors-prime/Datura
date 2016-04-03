using UnityEngine;
using System.Collections;

public class SkeletonSwordsman : Enemy
{
	// Use this for initialization
	protected override void Initialize()
	{
		// Character attributes
		characterName = "Skeleton Swordsman";
		maxHealth = 100.0f;
		currentHealth = 100.0f;
		attackDelay = 2.0f;
		damage = 5.0f;
		patrolRange = 10.0f;
		chaseRange = 12.0f;
		attackRange = 3.0f;
		attackEventTime = 0.37f;
		bleedWhenDamaged = false;
	}
}