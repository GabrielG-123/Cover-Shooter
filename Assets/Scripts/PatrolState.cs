using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State
{
    public StateManager manager;
    int currentwaypointindex = 0;
    

    public PatrolState(StateManager stateManager) : base(stateManager)
    {
       manager = stateManager;
    }

    public override State RunCurrentState()
    {

       

        manager.agent.destination = manager.waypoints[currentwaypointindex].position;

        
        manager.animator.SetBool("onPatrol", true);

       
        //Debug.Log("Current waypoint index: " + currentwaypointindex);

        if (manager.agent.remainingDistance <= 0.5f && !manager.agent.pathPending)
        {

          
            currentwaypointindex++;
          //  Debug.Log("Current waypoint index: " + currentwaypointindex);
            if (currentwaypointindex > manager.waypoints.Length - 1)
            {
                currentwaypointindex = 0;
                


            }
        

            return manager.idleState;

        }


        if (manager.alertMeter == 100 || manager.attacking) {

            if (manager.attacking == false) {
                manager.attacking = true;
            
            }

            return manager.attackState;        
        
        }
        return this;



        
    }

    public override void OnExit()
    {
        manager.animator.SetBool("onPatrol", false);
    }


   
}
