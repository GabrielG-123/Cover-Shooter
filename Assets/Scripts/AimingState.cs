using UnityEngine;

public class AimingState : State
{

    StateManager manager;
    public float rotationTimer;
    float timeToTurn = 0.5f;

    public AimingState(StateManager stateManager) : base(stateManager)
    {


        manager = stateManager;
    
    }


    public override void OnEnter(){
      
        Debug.Log("Entered the aiming state");
        manager.animator.SetLayerWeight(1, 1f);
    
     
    
    }
public override State RunCurrentState()
    {


        if (manager.coverScript.CheckCoverConditions() && !manager.coverScript.foundCover) {

            manager.coverScript.FindCover();
        
        }

        if (manager.coverScript.CheckCoverConditions() && !manager.coverScript.inCover) {

            manager.coverScript.MovingToCover();
        
        
        }

        Debug.Log("Aiming at the player");
        rotationTimer += Time.deltaTime;
        
        
        manager.agent.transform.rotation = Quaternion.Slerp(manager.agent.transform.rotation, manager.targetRotation, rotationTimer / timeToTurn);

        if (Vector3.Distance(manager.agent.transform.position, manager.player.transform.position) <= 30.0f)
        {

            return manager.attackState.shootingState;

        }

        else {

            return null;
        
        
        }


            
    }



}

 






