using UnityEngine;
using System.Collections;

public abstract class Enemy : Character 
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
	protected bool bleedWhenDamaged;
	public bool startSinking = false;
	public float timer = 0.0f; // Begin timer at this value

	public GameObject bloodPrefab;
	private GameObject bloodPrefabInstance;
    private float attackDelay;
	Animator animator; // Animator controls the currently playing animation of enemy
	
	// Enemy state
	public enum ENEMY_STATE {IDLE = 0, PATROL = 1, CHASE = 2, ATTACK = 3, BATTLE_STANCE = 4, DEAD = 5, FLEE = 6};
	protected enum ANIMATION_STATE { IDLE = 0, MOVE = 1, ATTACK = 2, DEAD = 3, TAKE_DAMAGE = 4 }
	public ENEMY_STATE activeState = ENEMY_STATE.PATROL;

	public float sinkSpeed = 2.5f; // Speed at which enemy sinks when dying
    public bool dead = false;

	protected abstract void Initialize();

	// Function is called when game scene loads
	void Start()
	{
		animator = GetComponent<Animator>();
        //attackDelay = animator.
		Initialize();
		player = PlayerController.player;
		ChangeState(activeState);
	}

	// Function is called on every in-game frame
	void Update()
	{
		if(startSinking) transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
	}

	// Function is called when user hovers over game object
	void OnMouseEnter()
	{
		if(activeState != ENEMY_STATE.DEAD) 
		{
			player.target = this;
			HealthBar.enableHealthBar();
		}
	}

	// Function is called when user stops hovering on game object
	void OnMouseExit() 
	{
		if(activeState != ENEMY_STATE.DEAD) 
		{
			player.target = null;
			HealthBar.disableHealthBar();
		}
	}

	// Function is called when enemy takes damage
	public void TakeDamage(float damage)
	{
		currentHealth-= damage; // Reduce enemy health

		if(currentHealth <= 0) ChangeState(ENEMY_STATE.DEAD); // Play dead animation
		else if(Random.Range(0.0f, 1.0f) < 0.2f) animator.SetInteger("CharacterState", (int) ANIMATION_STATE.TAKE_DAMAGE); // Play get hit animation

		audioSource.PlayOneShot(impactSound, 0.7F); // Play get hit sound
		if(bleedWhenDamaged) bloodPrefabInstance = Instantiate(bloodPrefab, transform.position, Quaternion.identity) as GameObject;
	}

	// Function is called when enemy attacks
	protected void CauseDamage()
	{
		// If player is within attack range, damager player
		if(Vector3.Distance(transform.position, player.transform.position) < attackRange)
			player.TakeDamage(damage);
	}

	// Change state of the enemy in state machine
	protected void ChangeState(ENEMY_STATE state)
	{
		StopAllCoroutines();

		if(activeState != ENEMY_STATE.DEAD)
		{
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
	}

	// Initiate enemy idle state
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

	// Initiate enemy patrol state
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

	// Initiate enemy chase state
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

	// Initiate enemy attack state
	IEnumerator EnemyAttack()
	{
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.ATTACK);
		navAgent.Stop();
		//float elapsedTime = attackDelay;

		while(activeState == ENEMY_STATE.ATTACK)
		{
			//elapsedTime += Time.deltaTime;
			transform.LookAt(player.transform); // Face player when attacking

			// Calculate distance to player
			float playerDistance = Vector3.Distance(transform.position, player.transform.position);

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

			yield return null;
		}
	}

	// Initiate enemy flee state
	IEnumerator EnemyFlee()
	{
		//temporarily point the object to look away from the player
		transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
		
		//Then we'll get the position on that rotation that's multiplyBy down the path (you could set a Random.range
		// for this if you want variable results) and store it in a new Vector3 called runTo
		Vector3 runTo = transform.position + transform.forward * 5.0f;

		NavMeshHit hit; // stores the output in a variable called hit
		NavMesh.SamplePosition(runTo, out hit, 100.0f, 1);

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
		
	// Modify the currently playing animation of the enemy
	void setAnimationState(ANIMATION_STATE animationState)
	{
		animator.SetInteger("CharacterState", (int) animationState);
	}

	// Initiate enemy dead state
	IEnumerator EnemyDead()
	{
        if (!dead)
        {
            dead = true;
            animator.SetInteger("CharacterState", (int)ANIMATION_STATE.DEAD); // Play dead animation 
        }

        //transform.parent = null; //set parent (Squadron reference) to null, 

		Destroy(navAgent); // Make game object immovable
		Destroy(GetComponent<Collider>()); // Make game object non selectable
		if(player.target == this) player.target = null; // Unset this game object as target

		float sinkingTime = 5.0f;
        //float timerMax = 10.0f; // Destroy game oxbject once timer hits this value
		// Delete game object once timer hits maximums
		while(timer < 6.0f)
		{
			timer += Time.deltaTime;
			if(timer >= sinkingTime) startSinking = true;
			yield return null;
		}

		Destroy(gameObject);
		yield return null;
	}
}