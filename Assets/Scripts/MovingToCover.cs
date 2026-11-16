using UnityEngine;

public class MovingToCover : State
{
    private StateManager manager;
    private Vector3 coverDestination;
    public MovingToCover(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }
    public override State RunCurrentState()
    {

        MoveToCover();

       coverDestination = manager.coverScript.FindCoverSpace();
        

        if (manager.coverScript.findNewSpace) { 
        
              
          //  manager.agent.destination = 
          
            manager.agent.destination = coverDestination;
        
        }

        if (manager.coverScript.inCover) {

            return manager.attackState.coverIdleState;


        
        
        }

        if (!manager.coverScript.hasStartedMoving)
        {

            Debug.Log("Checking if it is false");
            if (manager.agent.velocity.magnitude > 0.1f)
            {

                manager.coverScript.hasStartedMoving = true;
            }




        }

        else {
            if (manager.agent.velocity.magnitude <= 0.001f) {


                manager.coverScript.hasStartedMoving = false;
            
            }
        
        }

            return this;
    }

    public override void OnEnter()
    {

        

        coverDestination = manager.coverScript.FindCover();
        manager.agent.destination = coverDestination;



      //  Debug.Log("In the OnEnter");


       

    }

    public void MoveToCover() {





        if (manager.coverScript.hasStartedMoving && manager.agent.remainingDistance <= manager.agent.stoppingDistance && !manager.agent.pathPending)
        {
            Debug.Log("This is currently running for agent: " + manager.transform.name);
            if (manager.coverScript.closestCover == null) return;
            manager.coverScript.distanceToCover = manager.coverScript.closestCover.transform.position - manager.transform.position;

            if (Physics.Raycast(manager.agent.transform.position, manager.coverScript.distanceToCover, out manager.coverScript.coverHit, 5.0f))
            {
                manager.coverScript.oppositeNormal = -manager.coverScript.coverHit.normal;
                manager.coverScript.coverRotation = Quaternion.LookRotation(manager.coverScript.oppositeNormal, Vector3.up);
                Debug.Log("Doing this currently");
                manager.animator.SetBool("takeCover", true);
                Debug.Log("Yup, true");
                manager.coverScript.inCover = true;
            }
        }
        else
        {
            Debug.Log("This is now running for the agent: " + manager.transform.name);
        }



    }

}
