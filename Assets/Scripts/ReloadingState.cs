using UnityEngine;

public class ReloadingState : State
{
    private StateManager manager;

    public ReloadingState(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }

    public override State RunCurrentState()
    {


        if (manager.animator.GetBool("isReloading"))
        {
            return this;
        }



        return manager.attackState.aimingState;




    }


    


}
