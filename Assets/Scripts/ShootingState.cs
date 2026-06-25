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
        //manager.firing = true;



        manager.RigLayers[0].active = true;
    }


    public void AggroMovement() {
        if (!manager.coverScript.isDefensive && !manager.coverScript.inCover) {         
            if(manager.distanceFromPlayer >= 10.0f && manager.distanceFromPlayer <= 20.0f)
            {

                if (manager.coverScript.CheckCoverConditions())
                {
                    Debug.Log("I am not aggroing the player");
                    manager.coverScript.isAggro = false;



                }

                else
                {


                    manager.coverScript.isAggro = true;


                }
                if (manager.agent.remainingDistance <= manager.agent.stoppingDistance  && !manager.agent.pathPending)
            {
                aggroPoint = Random.insideUnitSphere * 10 + manager.player.transform.position;
                aggroPoint.y = manager.agent.transform.position.y;

                if (NavMesh.SamplePosition(aggroPoint, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                {
                    Vector3 realAggroPoint = navHit.position;
                    manager.agent.SetDestination(realAggroPoint);
                }
            }

            
            Debug.Log("I am moving to the aggro point: " + aggroPoint);

            manager.animator.SetBool("strafing", true);


               


            }
        }


    }

    public override State RunCurrentState()
    {

        AggroMovement();

        if (manager.coverScript.CheckCoverConditions() && !manager.coverScript.inCover && !manager.coverScript.foundCover)
        {
            Debug.Log("The status of CheckCoverConditions is: " + manager.coverScript.CheckCoverConditions());
            Debug.Log("Yes, this is actually happening");
            manager.coverScript.FindCover();

        }

        if (manager.coverScript.CheckCoverConditions() && !manager.coverScript.inCover)
        {
            Debug.Log("I am moving to cover");
            manager.coverScript.MovingToCover();


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

        if (manager.distanceFromPlayer >= 20.0f && !manager.coverScript.foundCover) {
          //  Debug.Log("I am no longer shooting at the player");

            manager.onSearch = true;
            return null;

           
           
           
           
           //return manager.patrolState;
        
        
        }

        return this;
    }

    public override void OnExit() {
       // Debug.Log("This will run");

        manager.animator.SetBool("isAiming", false);
        manager.firing = false;
      manager.RigLayers[0].active = false;
    
    }
}
