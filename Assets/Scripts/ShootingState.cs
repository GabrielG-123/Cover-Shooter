using UnityEngine;

public class ShootingState : State
{
    private StateManager manager;
    float timeToTurn = 0.5f;
    float rotationTimer;
    //float burstShotsFired = 0;
    float timeSinceLastShot = 0;


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

        manager.RigLayers[0].active = true;
    }

    public override State RunCurrentState()
    {


        if (manager.coverScript.checkCoverConditions() && !manager.coverScript.foundCover)
        {

            manager.coverScript.FindCover();

        }

        if (manager.coverScript.checkCoverConditions() && !manager.coverScript.inCover)
        {

            manager.coverScript.MovingToCover();


        }


        if (manager.coverScript.inCover)
        {

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
            if (manager.burstShotsFired < manager.myGun.burstRounds && !manager.coverScript.enteringCover)
            {
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

        if (Vector3.Distance(manager.agent.transform.position, manager.player.transform.position) >= 20.0f) {
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
