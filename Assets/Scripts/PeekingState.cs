using UnityEngine;

public class PeekingState : State
{
    public StateManager manager;

    public PeekingState(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }

    public override State RunCurrentState()
    {

        if(!manager.coverScript.coverPeek) {
            return manager.attackState.coverIdleState;
        }
        return this;
    }


    public override void OnEnter()
    {
        manager.RigLayers[0].active = true;
        manager.animator.SetBool("isPeeking", true);


    }
}
