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
            if (manager.coverScript.CheckCoverConditions() && !manager.coverScript.inCover)
            {
                if (!manager.coverScript.isAggro)
                {
                    Debug.Log("I am moving to cover as agent: " + manager.transform.name);
                    manager.coverScript.MovingToCover();

                }

            }
            return this;
        }



        return manager.attackState.aimingState;




    }
}
