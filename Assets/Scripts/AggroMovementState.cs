using UnityEngine;

public class AggroMovementState : State
{
    public StateManager manager;
    public State currentSubState;
    public AggroMovementState(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }

    public override State RunCurrentState()
    {
        return this;
    }

    
}
