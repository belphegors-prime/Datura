using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public static PlayerController player; // Reference to player
	public Enemy target; // Reference to enemy target
	public Enemy lastTarget; // Reference to last selected enemy
    public GameObject ground;
    public Canvas inventoryPrefab;

    GameObject rightHandWeap, leftHandWeap, leftHandShieldMount;
    
    public GameObject Sword2H, Sword1H, Shield;
	Animator animator;

    private Canvas inventInst;

	// Player attributes
	public float maxHealth;
	public float currentHealth;
	public float attackRange = 4.0f;
	public float playerDamage = 35.0f;
    public float bufDistToTarget = .5f;

	private bool deathAnimationCompleted = false;
	public bool attackAnimationCompleted = false;

   // public Animator anim1H, anim2H;
	
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

        //rightHandWeap = GameObject.FindWithTag("Test");
	}
	
	public void Update()
	{
		HandleButtonClick(); // If user clicks left mouse button, handle click
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
		ChangeState(PLAYER_STATE.IDLE);
	}

	// Reduce player health
	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		if(currentHealth <= 0) ChangeState(PLAYER_STATE.DEAD);
	}

    public void DisableInventoryResumeGame()
    {
        Time.timeScale = 1;
        //inventoryPrefab.enabled = false;
        GameObject i = GameObject.Find("ProtoInventory");
        i.SetActive(false);
        if (!inventoryPrefab.enabled) Debug.Log("???? fml");
    }
    
    public void SwitchTo2H()
    {
        GameObject currentWeap = rightHandWeap.transform.GetChild(0).gameObject;
        currentWeap.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        
        //GameObject currentShield = leftHandShieldMount.transform.GetChild(0).gameObject;
       // Destroy(currentShield.gameObject);
        
        //GameObject sword2Inst = Instantiate(Sword2H, rightHandWeap.transform.position, Quaternion.identity) as GameObject;
        
        //sword2Inst.transform.SetParent(rightHandWeap.transform);
        //sword2Inst.transform.Rotate(new Vector3(0, 90, 270));

        animator.runtimeAnimatorController = Resources.Load("2H_WeaponControl") as UnityEngine.RuntimeAnimatorController;
        
    }
    
	// If user presses button, handle interaction
	private void HandleButtonClick()
	{
		// If user presses left mouse button, move player to where raycast hits terrain
        if(player.activeState != PLAYER_STATE.DEAD){

		    if(Input.GetMouseButtonUp (0)) 
		    {
			    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			    Physics.Raycast (ray, out raycastHit, 100);

			    if (!target) ChangeState (PLAYER_STATE.MOVE); // If no target is specified, move
			    else ChangeState (PLAYER_STATE.ATTACK); // If target is enemy specified, attack
		    } 

		
            else if(Input.GetButtonUp("Inventory"))
            {
                Debug.Log("inventory button up");
                PauseGame();
                inventoryPrefab.gameObject.SetActive(true);
                //inventInst = Instantiate(inventoryPrefab) as Canvas;
                //inventInst.enabled = true;
            }
        }
        else if(deathAnimationCompleted && Input.GetMouseButtonUp (0)) {
			//Application.LoadLevel (Application.loadedLevelName);
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
    }

	public void ChangeState(PLAYER_STATE state)
	{
		StopAllCoroutines();
		activeState = state;

		switch(activeState)
		{
			case PLAYER_STATE.IDLE: StartCoroutine(PlayerIdle()); return;
			case PLAYER_STATE.MOVE: StartCoroutine(PlayerMove()); return;
			case PLAYER_STATE.ATTACK: StartCoroutine(PlayerAttack()); return;
			case PLAYER_STATE.DEAD: StartCoroutine(PlayerDead()); return;
		}
	}

	IEnumerator PlayerIdle()
	{
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.IDLE);
		yield return null;
	}

	IEnumerator PlayerDead()
	{
		animator.SetInteger("CharacterState", (int) ANIMATION_STATE.DEAD);
        Vector3 deathPos = player.transform.position;
        deathPos.y = -1.4f;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("die"))
        {
            //Debug.Log("inside if" + deathPos.y);
            setDeathAnimationCompleted();
            player.transform.position.Set(deathPos.x, deathPos.y, deathPos.z);//set player onto ground
        }
		yield return null;
	}

	IEnumerator PlayerMove()
	{
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
                navAgent.ResetPath();
                ChangeState(PLAYER_STATE.IDLE);
            }
			//}

			yield return null;
		}
	}

	IEnumerator PlayerAttack()
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
                CauseDamage();
			}


			// If player is within attack range, attack enemy
			else if(Vector3.Distance(transform.position, lastTarget.transform.position) <= attackRange)
			{
                Debug.Log("Attack anims start");
				transform.LookAt(targetGameObject.transform); // Face enemy when attacking
				navAgent.ResetPath(); // Stop moving player
				animator.SetInteger("CharacterState", (int) ANIMATION_STATE.ATTACK);
                setAttackAnimationCompleted();
			}

			yield return null;
		}
	}

}