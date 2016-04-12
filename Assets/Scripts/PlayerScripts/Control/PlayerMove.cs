using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour 
{
	/*public static PlayerMove player; // Reference to player
	public Enemy target; // Reference to enemy target
	public Enemy lastTarget; // Reference to last selected enemy
	
	Animator animator;

	// Player attributes
	public float maxHealth;
	public float currentHealth;
	public float attackRange = 4.0f;
	public float playerDamage = 35.0f;

    public float bufDistToTarget = .5f;

	private bool deathAnimationCompleted = false;
	public bool attackAnimationCompleted = false;
	
	// Player state
	public enum ANIMATION_STATE { IDLE = 0, MOVE = 1, ATTACK = 2, DEAD = 3 }
	public enum PLAYER_STATE {IDLE = 0, MOVE = 1, ATTACK = 2, DEAD = 3};
	public PLAYER_STATE activeState = PLAYER_STATE.IDLE;
	
	RaycastHit raycastHit; // Point where raycast hits
	NavMeshAgent navAgent; // Component that moves player

	public void Awake()
	{
		player = this;
	}

	public void Start()
	{
		animator = GetComponent<Animator>(); // Get component that plays animations
		navAgent = GetComponent<NavMeshAgent>(); // Get component that moves player
		ChangeState(activeState); // Set default state
		maxHealth = 100.0f;
		currentHealth = maxHealth;
	}
	
	public void Update()
	{
		HandleMouseClick(); // If user clicks left mouse button, handle click
	}

	public float getCurrentHealth()
	{
		return currentHealth;
	}

	public float getMaxHealth()
	{
		return maxHealth;
	}

	public void setAttackAnimationCompleted()
	{
		attackAnimationCompleted = true;
	}

	public void setDeathAnimationCompleted()
	{
		deathAnimationCompleted = true;
	}
	
	// Function triggers once attack animation reaches specific frame
	public void CauseDamage()
	{
		lastTarget.TakeDamage(playerDamage);
		//ChangeState(PLAYER_STATE.IDLE);
	}

	// Reduce player health
	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		if(currentHealth <= 0) ChangeState(PLAYER_STATE.DEAD);
	}

	// If user presses left mouse button, handle interaction
	private void HandleMouseClick()
	{
        // If user presses left mouse button, move player to where raycast hits terrain
        if (activeState != PLAYER_STATE.DEAD && Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out raycastHit, 100);

            if (!target) ChangeState(PLAYER_STATE.MOVE); // If no target is specified, move
            else ChangeState(PLAYER_STATE.ATTACK); // If target is enemy specified, attack
        }

        else if (deathAnimationCompleted && Input.GetMouseButtonUp(0)) { }
			//Application.LoadLevel (Application.loadedLevelName);
	}

	public void ChangeState(PLAYER_STATE state)
	{
		StopAllCoroutines();
		activeState = state;

		switch(activeState)
		{
			case PLAYER_STATE.IDLE: StartCoroutine(Idle()); return;
			case PLAYER_STATE.MOVE: StartCoroutine(Move()); return;
			case PLAYER_STATE.ATTACK: StartCoroutine(Attack()); return;
			case PLAYER_STATE.DEAD: StartCoroutine(Dead()); return;
		}
	}

	IEnumerator Idle()
	{
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.IDLE);
		yield return null;
	}

	IEnumerator Dead()
	{
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.DEAD);
		yield return null;
	}

	IEnumerator Move()
	{
		//bool runStarted = false;
        //float velocityThreshold = 0.5f; // Velocity threshold determining when player is running

        while (activeState == PLAYER_STATE.MOVE)
        {
            navAgent.SetDestination(raycastHit.point); // Move player to destination
            animator.SetInteger("CharacterState", (int)ANIMATION_STATE.MOVE);

            // If player passed velocity threshold, run has started
            //if(navAgent.velocity.magnitude > velocityThreshold) runStarted = true;

            // If player started his run and then went below velocity threshold, set state to idle
            //if(runStarted && navAgent.velocity.magnitude < velocityThreshold)
            //{
            if (Vector3.Distance(transform.position, raycastHit.point) < bufDistToTarget) { 
                runStarted = false;
                navAgent.ResetPath();
                ChangeState(PLAYER_STATE.IDLE);
            }
			//}

			yield return null;
		}
	}

	IEnumerator Attack()
	{
		lastTarget = target;
		GameObject targetGameObject = target.gameObject;

		while(activeState == PLAYER_STATE.ATTACK)
		{	
			// If player is not within attack range, move player towards enemy
			if(Vector3.Distance(transform.position, lastTarget.transform.position) > attackRange + 1)
			{
				navAgent.SetDestination(targetGameObject.transform.position);
				animator.SetInteger("CharacterState", (int) ANIMATION_STATE.MOVE);
			}

			else if(animator.GetInteger("CharacterState") == (int) ANIMATION_STATE.ATTACK && attackAnimationCompleted)
			{
				attackAnimationCompleted = false;
				ChangeState(PLAYER_STATE.IDLE);
			}


			// If player is within attack range, attack enemy
			else if(Vector3.Distance(transform.position, lastTarget.transform.position) <= attackRange)
			{
				transform.LookAt(targetGameObject.transform); // Face enemy when attacking
				navAgent.ResetPath(); // Stop moving player
				animator.SetInteger("CharacterState", (int) ANIMATION_STATE.ATTACK);

			}

			yield return null;
		}
	}*/
}