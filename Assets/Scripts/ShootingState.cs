using UnityEngine;
using UnityEngine.AI;

public class ShootingState : State
{
    private StateManager manager;
    float timeToTurn = 0.5f;
    float rotationTimer;
    //float burstShotsFired = 0;
    float timeSinceLastShot = 0;
    private Vector3 aggroPoint;
    public float aggroTimer;
    public float aggroDuration;
    public bool arrivedAtAggroPoint;
    private bool hasPreviousPath = false;


    public ShootingState(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }


    public override void OnEnter()
    {
        rotationTimer = manager.attackState.aimingState.rotationTimer;
        manager.agent.ResetPath();
        //   Debug.Log("Entering shooting state");
        manager.animator.SetBool("isAiming", true);
        manager.animator.SetBool("isShooting", true);
        manager.agent.speed = 2.5f;
        //manager.firing = true;



        manager.RigLayers[0].active = true;
    }


    public void AggroMovement()
    {

       
        // Only evaluate entering aggro if we aren't already locked into a path.
        // If we have a path, we are committed to finishing it.
        if (!hasPreviousPath)
        {

            bool inRange = manager.distanceFromPlayer >= 5.0f && manager.distanceFromPlayer <= 15.0f;
            manager.coverScript.isAggro = inRange && !manager.coverScript.CheckCoverConditions();
        }

        // Exit early if we are not aggro, or if a high-priority defensive override happens
        if (!manager.coverScript.isAggro || manager.coverScript.inCover || manager.coverScript.foundCover)
        {
            
            return;
        }

        Debug.Log("Still here");

        // 1. If we don't have a destination yet, find one
        if (!hasPreviousPath)
        {
            Debug.Log("Here we are");
            SetNewAggroDestination();
            
        }
        else 
        {
            // 2. Check if the agent has physically arrived at the point
            if (!manager.agent.pathPending && manager.agent.remainingDistance <= manager.agent.stoppingDistance)
            {
                if (!arrivedAtAggroPoint)
                {
                    // Just arrived this exact frame
                    Debug.Log("On this frame");
                    arrivedAtAggroPoint = true;
                    aggroDuration = Random.Range(1.0f, 3.0f);
                    
                }
                else
                {
                    // 3. We are waiting at the point. Run the timer.
                    Debug.Log("Currently here");
                    aggroTimer += Time.deltaTime;

                    if (aggroTimer >= aggroDuration)
                    {
                        aggroTimer = 0f; 
                        // Time is up. Now we are allowed to re-evaluate conditions.
                        // bool inRange = manager.distanceFromPlayer >= 10.0f && manager.distanceFromPlayer <= 20.0f;
                        if (manager.coverScript.CheckCoverConditions())
                        {
                            manager.coverScript.isAggro = false;
                            hasPreviousPath = false; // Reset for next time we enter aggro
                        }
                        else
                        {
                            // Still in aggro, pick a new point and repeat
                            SetNewAggroDestination();
                        }
                    }
                }
            }
        }

        manager.animator.SetBool("strafing", true);
    }

    // Keep this helper method exactly the same as before
    private void SetNewAggroDestination()
    {
        Debug.Log("Setting new aggro destination");
        Vector2 randomCircle = Random.insideUnitCircle * 6.5f;
        aggroPoint = new Vector3(randomCircle.x, 0, randomCircle.y) + manager.player.transform.position;

        if (NavMesh.SamplePosition(aggroPoint, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
        {
            manager.agent.SetDestination(navHit.position);
            hasPreviousPath = true;
            arrivedAtAggroPoint = false;
           // aggroTimer = 0f;
        }
    }


    public override State RunCurrentState()
    {

        Debug.Log("Has a previous path: " + hasPreviousPath);

        AggroMovement();

        if (manager.coverScript.CheckCoverConditions() && !manager.coverScript.inCover && !manager.coverScript.foundCover)
        {
            if (!manager.coverScript.isAggro)
            {
                Debug.Log("The status of CheckCoverConditions is: " + manager.coverScript.CheckCoverConditions());
                Debug.Log("Yes, this is actually happening");
                manager.coverScript.FindCover();
            }
        }

        if (manager.coverScript.CheckCoverConditions() && !manager.coverScript.inCover)
        {
            if (!manager.coverScript.isAggro)
            {
                Debug.Log("I am moving to cover as agent: " + manager.transform.name);
                manager.coverScript.MovingToCover();

            }

        }


        if (manager.coverScript.inCover)
        {
            Debug.Log("The status of inCover is: " + manager.coverScript.inCover);  
            Debug.Log("Happening currently");
            manager.coverScript.coverShootCheck();



        }



        //if (manager.coverScript.checkExitCoverConditions()) {


        //    manager.coverScript.ExitCover();
        
        
        //}





        if (manager.myGun.ammoInClip == 0 && manager.myGun.ammoReserve != 0)
        {

          
            
            manager.myGun.reload(1);


        }

        //if (manager.healthScript.health <= 50) {

        //    return manager.attackState.coverState;
        
        
        
        //}

        


            rotationTimer += Time.deltaTime;
        if (!manager.coverScript.inCover)
        {
            manager.agent.transform.rotation = Quaternion.Slerp(manager.agent.transform.rotation, manager.targetRotation, rotationTimer / timeToTurn);
        }

         
        if (manager.firing == true && manager.animator.GetBool("isShooting"))
        {

            Debug.Log("here");
            if (manager.burstShotsFired < manager.myGun.burstRounds && !manager.coverScript.enteringCover)
            {

                Debug.Log("I am firing at the player");
                manager.myGun.Shoot();
                manager.burstShotsFired++;
                //Debug.Log("the burst shots fired: " + burstShotsFired);





            }

            else
            {
                timeSinceLastShot += Time.deltaTime;
                if (timeSinceLastShot >= manager.myGun.timeBetweenBursts)
                {

                    manager.burstShotsFired = 0;
                    timeSinceLastShot = 0;



                }





            }


          //  Debug.Log("I am shooting at the player");

        }

        if (manager.distanceFromPlayer >= 20.0f ) {
           Debug.Log("I am no longer shooting at the player");

            manager.onSearch = true;
            return null;

          
           
           
           
           //return manager.patrolState;
        
        
        }

        return this;
    }

    public override void OnExit() {
       // Debug.Log("This will run");

        manager.animator.SetBool("isAiming", false);
        manager.animator.SetBool("strafing", false);
        manager.animator.SetBool("isShooting", false);
        manager.firing = false;
      manager.RigLayers[0].active = false;
    
    }
}
