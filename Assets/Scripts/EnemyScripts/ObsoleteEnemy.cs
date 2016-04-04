using UnityEngine;
using System.Collections;

/*public abstract class ObsoleteEnemy : Character 
{
	// Enemy attributes
	public PlayerController player;
	protected float patrolRange;
	protected float chaseRange;
	protected float attackRange;
	protected float attackEventTime;
	protected float morale;
	protected float minimumFightingMorale;
	protected bool willFlee;
	
	Animator animator;
	
	// Enemy state
	protected enum ENEMY_STATE {IDLE = 0, PATROL = 1, CHASE = 2, ATTACK = 3, BATTLE_STANCE = 4, DEAD = 5, FLEE = 6};
	protected enum ANIMATION_STATE { IDLE = 0, MOVE = 1, ATTACK = 2, DEAD = 3, TAKE_DAMAGE = 4 }
	protected ENEMY_STATE activeState = ENEMY_STATE.PATROL;

	void Start()
	{
		animator = GetComponent<Animator>();
		Initialize();
		player = PlayerController.player;
		ChangeState(activeState);
	}

	void OnMouseEnter()
	{
		if(activeState != ENEMY_STATE.DEAD) 
		{
			//player.target = this;
			HealthBar.enableHealthBar();
		}
	}

	protected abstract void Initialize();

	void OnMouseExit() 
	{
		if(activeState != ENEMY_STATE.DEAD) 
		{
			player.target = null;
			HealthBar.disableHealthBar();
		}
	}

	public void TakeDamage(float damage)
	{
		// Play impact damage sound
		audioSource.PlayOneShot(impactSound, 0.7F);

		// Reduce enemy health
		currentHealth-= damage;
		if(currentHealth <= 0) ChangeState(ENEMY_STATE.DEAD);
		else animator.SetInteger("CharacterState", (int) ANIMATION_STATE.TAKE_DAMAGE); // Play dead animation


		// Reduce morale, flee if morale below minimum
//		else 
//		{
//			morale -= 50;
//			if(willFlee && morale < minimumFightingMorale) 
//			{
//				//Debug.Log("Flee enable");
//				ChangeState(ENEMY_STATE.FLEE);
//			}
//		}
	}

	protected void CauseDamage()
	{
		// If player is within attack range, damager player
		if(Vector3.Distance(transform.position, player.transform.position) < attackRange)
			player.TakeDamage(damage);
	}

	protected void ChangeState(ENEMY_STATE state)
	{
		StopAllCoroutines();
		activeState = state;

		switch(activeState)
		{
			case ENEMY_STATE.IDLE: StartCoroutine(EnemyIdle()); return;
			case ENEMY_STATE.PATROL: StartCoroutine(EnemyPatrol()); return;
			case ENEMY_STATE.CHASE: StartCoroutine(EnemyChase()); return;
			case ENEMY_STATE.ATTACK: StartCoroutine(EnemyAttack()); return;
			case ENEMY_STATE.FLEE: StartCoroutine(EnemyFlee()); return;
			case ENEMY_STATE.DEAD: StartCoroutine(EnemyDead()); return;
		}
	}

	IEnumerator EnemyIdle()
	{
		navAgent.Stop(); // Stop moving enemy;
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.IDLE);

		float elapsedTime = 0;
		float idleTime = Random.Range (0.0F, 5.0F);

		// Play idle animation while in idle state
		while(activeState == (int) ENEMY_STATE.IDLE) 
		{
			elapsedTime += Time.deltaTime;

			if(elapsedTime >= idleTime) ChangeState(ENEMY_STATE.PATROL);

			// Calculate distance to player
			float playerDistance = Vector3.Distance(transform.position, player.transform.position);
			
			// If player is within attack range, attack
			if(playerDistance < attackRange)
			{
				ChangeState(ENEMY_STATE.ATTACK);
				yield break;
			}
			
			// If player is outside chase range, chase
			else if(playerDistance < chaseRange)
			{
				ChangeState(ENEMY_STATE.CHASE);
				yield break;
			}

			yield return null;
		}
	}

	IEnumerator EnemyPatrol()
	{
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.MOVE);
		navAgent.Stop(); // Stop moving enemy

		// Get random destination on map
		Vector3 randomPosition = Random.insideUnitSphere * patrolRange;

		// Add as offset from current position
		randomPosition += this.transform.position;

		// Get nearest valid position
		NavMeshHit hit;
		NavMesh.SamplePosition(randomPosition, out hit, patrolRange, 1);

		// Set destination
		navAgent.SetDestination(hit.position);
		navAgent.Resume();
		animator.SetInteger("CharacterState", 1);

		// Set distance range between object and destination to classify as 'arrived'
		float arrivalDistance = 0.05f;

		// Set timeout before new path is generated (5 seconds)
		float timeOut = 5.0f;

		//Elapsed Time
		float elapsedTime = 0;

		// Wait until enemy reaches destination or times out, then get new position
		while(Vector3.Distance(transform.position, hit.position) > arrivalDistance 
			&& elapsedTime < timeOut)
		{
			// Update elapsed time
			elapsedTime += Time.deltaTime;

			// Check if should enter chase state
			if(Vector3.Distance(this.transform.position, player.transform.position) < chaseRange)
			{
				ChangeState(ENEMY_STATE.CHASE);
				yield break;
			}

			yield return null;
		}

		navAgent.Stop(); // Stop moving enemy
		ChangeState(ENEMY_STATE.IDLE);
	}

	IEnumerator EnemyChase()
	{
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.MOVE);
		navAgent.Stop(); // Stop moving enemy
		navAgent.SetDestination(player.transform.position); // Chase player
		navAgent.Resume(); // Stop moving enemy
		animator.SetInteger("CharacterState", 1);

		while(activeState == ENEMY_STATE.CHASE) 
		{
			navAgent.SetDestination(player.transform.position); // Chase player

			// Calculate distance to player
			float playerDistance = Vector3.Distance(transform.position, player.transform.position);
			
			// If player is within attack range, attack
			if(playerDistance < attackRange)
			{
				ChangeState(ENEMY_STATE.ATTACK);
				yield break;
			}
			
			// If player is outside chase range, patrol
			else if(playerDistance > chaseRange)
			{
				ChangeState(ENEMY_STATE.IDLE);
				yield break;
			}
		
			yield return null; // Wait until next frame
		}
	}

	IEnumerator EnemyAttack()
	{
		navAgent.Stop();
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.ATTACK);

		float elapsedTime = attackSpeed;

		while(activeState == ENEMY_STATE.ATTACK)
		{
			elapsedTime += Time.deltaTime;

			transform.LookAt(player.transform); // Face player when attacking

			// Calculate distance to player
			float playerDistance = Vector3.Distance(transform.position, player.transform.position);

			if(morale < minimumFightingMorale)

			// If player is not within chase range, patrol
			if(playerDistance > chaseRange)
			{
				ChangeState(ENEMY_STATE.PATROL);
				yield break;
			}

			// If player is not within attack range, chase
			if(playerDistance > attackRange)
			{
				ChangeState(ENEMY_STATE.CHASE);
				yield break;
			}

			if(elapsedTime >= attackSpeed) elapsedTime = 0.0f;

			yield return null;
		}
	}

	IEnumerator EnemyFlee()
	{
		//temporarily point the object to look away from the player
		transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
		
		//Then we'll get the position on that rotation that's multiplyBy down the path (you could set a Random.range
		// for this if you want variable results) and store it in a new Vector3 called runTo
		Vector3 runTo = transform.position + transform.forward * 5.0f;

		NavMeshHit hit; // stores the output in a variable called hit
		NavMesh.SamplePosition(runTo, out hit, 100.0f, 1);

		//Debug.Log(hit.position);

		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.MOVE);
		navAgent.Resume();

		// And get it to head towards the found NavMesh position
		float regularEnemySpeed = navAgent.speed;
		navAgent.speed = 40.0f; 
		navAgent.SetDestination(hit.position);

		InvokeRepeating("IncreaseMorale", 1.0f, 1.0f);

		while(activeState == ENEMY_STATE.FLEE)
		{
			if(morale >= minimumFightingMorale) 
			{
				CancelInvoke("IncreaseMorale");
				navAgent.speed = regularEnemySpeed;
				ChangeState(ENEMY_STATE.IDLE);
			}
			yield return null;
		}
	}

	void IncreaseMorale()
	{
		//Debug.Log("increase morale triggers");
		morale++;
	}

	void setAnimationState(ANIMATION_STATE animationState)
	{
		animator.SetInteger("CharacterState", (int) animationState);
	}
	
	IEnumerator EnemyDead()
	{
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.DEAD); // Play dead animation
		Destroy(navAgent); // Make game object immovable
		Destroy(GetComponent<Collider>()); // Make game object non selectable
		if(player.target == this) player.target = null; // Unset this game object as target

		float timer = 0.0f; // Begin timer at this value
		float timerMax = 10.0f; // Destroy game oxbject once timer hits this value

		// Delete game object once timer hits maximums
		while(activeState == ENEMY_STATE.DEAD) 
		{
			timer += Time.deltaTime;
			if(timer >= timerMax) Destroy(gameObject);
			yield return null;
		}
	}
}*/