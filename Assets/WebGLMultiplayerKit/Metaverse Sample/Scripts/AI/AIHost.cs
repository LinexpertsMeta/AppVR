using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MetaverseSample{
public class AIHost : MonoBehaviour
{
    bool isWalk;
    public int  currentPoint;
    public float walkDelay;

    public Transform[] walkPoints;
    Animator myAnim;
    public NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding             

	public Transform target;// the target ofEnemyAI

    public float maxDistance;
	
    


    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent <Animator> ();
		agent = GetComponent <NavMeshAgent> ();
        
    }

    // Update is called once per frame
    void Update()
    {
      
      //  Turning();
      if(isWalk)
		{
			
			target = walkPoints[currentPoint];
			agent.enabled = true;
			agent.destination = new Vector3 (target.position.x, 
				transform.position.y, target.position.z);

            UpdateAnimator("IsWalk");
		}
        else
        {
            agent.enabled = false;
            UpdateAnimator("IsIdle");
			

        }
        StartCoroutine ("ProcessWalk");
        
    }

    /// <summary>
	/// Tries the atack.
	/// </summary>
	private IEnumerator ProcessWalk()
	{
		
		if (isWalk )
		{
			yield break; 
		}

      

        isWalk  = false;

	
		yield return new WaitForSeconds(walkDelay);

        currentPoint = UnityEngine.Random.Range(0, walkPoints.Length);

        isWalk = true;
			
	
	}

    

    public void UpdateAnimator(string _animation)
	{

	
			switch (_animation) { 
			case "IsWalk":
				if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Walk"))
				{
					myAnim.SetTrigger ("IsWalk");
				}
				break;


			case "IsIdle":

				if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
				{
					myAnim.SetTrigger ("IsIdle");
				}
				break;

				
		
			

			}
	
	}

    //makes enemy look to target
	void Turning()
	{
       
		if(NetworkManager.instance.localPlayer)
		{
			
				
			Vector3 meToPlayer = NetworkManager.instance.localPlayer.transform.position - transform.position;

		    meToPlayer.y = 0f;

		    transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (meToPlayer)
            , Time.deltaTime * 3f);
		  

			
		}

	}

 
}
}
