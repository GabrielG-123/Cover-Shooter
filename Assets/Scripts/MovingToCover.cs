using UnityEngine;

public class MovingToCover : State
{
    private StateManager manager;
    public MovingToCover(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }
    public override State RunCurrentState()
    {
        return this;
    }

    
}
