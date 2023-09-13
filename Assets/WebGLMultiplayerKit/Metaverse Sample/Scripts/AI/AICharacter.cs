using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
namespace MetaverseSample{
public class AICharacter : MonoBehaviour {

    public NavMeshAgent agent;
	public Transform[] targets;
	public int currentIndex;
	public Transform nextPoint;
	public Vector3 meToTarget = Vector2.zero;
	public float maxDistance = 1f;

	public float maxDistanceToPlayer = 5f;

    public bool isIdle;

	public int direction ;

	
	// Use this for initialization
	void Start () {
	
		agent = GetComponent <NavMeshAgent> ();
		direction = 1;
		currentIndex = 1;
		nextPoint = targets[1];
	

	}
	
	// Update is called once per frame
	void Update () {
	
	 
	   Move();
	   OnCustomCollider();
	}
	
	/// method for detecting the proximity of the player to the LootBox
	/// </summary>
	void Move()
	{


		Vector3 meToTarget = transform.position - nextPoint.position;


		if(!isIdle)
		{
			agent.destination = new Vector3 (nextPoint.position.x, 
			                      transform.position.y, nextPoint.position.z);

			TurningToTarget();
		

		}

			 

		//check if player is near
		if (meToTarget.sqrMagnitude < maxDistance) 
		{ 

			if (currentIndex >= targets.Length-1) {
				currentIndex = 0;

			}
			currentIndex += direction;
			nextPoint = targets [currentIndex];
				
		}

		
    }

		/// method for detecting the proximity of the player to the LootBox
	/// </summary>
	void OnCustomCollider()
	{

		
		if(NetworkManager.instance.localPlayer)
		{
			
			
			Vector3 meToPlayer = transform.position - NetworkManager.instance.localPlayer.transform.position;

		
			//check if player is near
			if (meToPlayer.sqrMagnitude < maxDistanceToPlayer) 
			{ 
				isIdle = true;
				agent.enabled = false;
				TurningToPlayer();
			
			}

			else
			{
				isIdle = false;
				agent.enabled = true;
				
			}

		}	
		
			
    }

	  //makes enemy look to target
	void TurningToPlayer()
	{
       
		if(NetworkManager.instance.localPlayer!=null)
		{
			
				
			Vector3 meToPlayer = NetworkManager.instance.localPlayer.transform.position - transform.position;

		    meToPlayer.y = 0f;

		    transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (meToPlayer)
            , Time.deltaTime * 3f);
		  

			
		}

	}

	//makes enemy look to target
	void TurningToTarget()
	{
       
		
		Vector3 meToTarget = nextPoint.position - transform.position;

		meToTarget.y = 0f;

		transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (meToTarget )
            , Time.deltaTime * 3f);
		  

	}


	
}//END_CLASS
}//END_NAMESPACE
