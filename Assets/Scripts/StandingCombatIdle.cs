using UnityEngine;

public class StandingCombatIdle : State
{
    public StateManager manager;
    public StandingCombatIdle(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }


    public override State RunCurrentState()
    {
        return this;
    }




}
