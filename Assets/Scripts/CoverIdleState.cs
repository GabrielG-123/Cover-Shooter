using UnityEngine;

public class CoverIdleState : State
{
    private StateManager manager;

    public CoverIdleState(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }

    public override State RunCurrentState()
    {

        if (manager.coverScript.coverShooting) {

            manager.RigLayers[0].active = false;



          }

        if (manager.coverScript.coverPeek) { 
        
                    return manager.attackState.peekingState;



        }
        



        return this;

    }

    public override void OnEnter()
    {

        manager.RigLayers[0].active = false;

    }

    public override void OnExit()
    {
        
    }
}
