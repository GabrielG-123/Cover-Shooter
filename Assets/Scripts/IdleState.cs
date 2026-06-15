using UnityEngine;

public class IdleState : State
{


    public StateManager manager;
   public float idleTime;
    

    public IdleState(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }


    public override void OnEnter()
    {
        manager.animator.SetBool("isIdle", true);
    }


    public override State RunCurrentState()
    {

       // Debug.Log("This state is running constantly");
        idleTime += Time.deltaTime;




        if (idleTime > 5.0f) {


            idleTime = 0;



            return manager.patrolState;



        }


        if (manager.attacking || manager.alertMeter == 100) {


            
            return manager.attackState;
        
        
        }

        else
        {

            return this;


        }


       




            
    }

    public override void OnExit() {
        manager.animator.SetBool("isIdle", false);
    
    
    }
}
