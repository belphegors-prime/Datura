//using UnityEngine;
//using System.Collections;
//
//public class ClickToMove : MonoBehaviour 
//{
//	NavMeshAgent navAgent;
//	Animation animation;
//	public AnimationClip idleAnimation;
//	public AnimationClip runAnimation;
//
//	// Use this for initialization
//	void Start() 
//	{
//		navAgent = GetComponent<NavMeshAgent> ();
//		animation = GetComponent<Animation> ();
//	}
//	
//	// Update is called once per frame
//	void Update() 
//	{
//		move();
//		animate();
//	}
//
//	// Move player to location of mouse click on terrain
//	void move()
//	{
//		if(!Player.isAttacking)
//		{
//			RaycastHit hit; // Point where raycast hits terrain
//			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); // Create raycast each frame
//
//			// If user presses left mouse button, move player to where raycast hits terrain
//			if(hit.transform.gameObject.GetComponents<Enemy>().Length != null && Input.GetMouseButtonUp (0) 
//			   && Physics.Raycast (ray, out hit, 100)) 
//			{
//				navAgent.SetDestination(hit.point);
//			}
//		}
//	}
//
//	// If velocity is larger than 0.5f play run animation. Otherwise play standing animation
//	void animate()
//	{
//		if(!Player.isAttacking)
//		{
//			if(navAgent.velocity.magnitude > 0.5f) animation.CrossFade(runAnimation.name);
//			else animation.CrossFade(idleAnimation.name);
//		}
//	}
//}