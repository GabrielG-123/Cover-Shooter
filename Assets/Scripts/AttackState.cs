using UnityEngine;

public class AttackState : State
{
    public StateManager manager;
   public State currentSubState;
    public State currentCombatState;
    public State currentMovementState;
    public AimingState aimingState;
    public ReloadingState reloadingState;
    public ShootingState shootingState;
    public CoverIdleState coverIdleState;
    public AggroMovementState aggroMovementState;
    public StandingCombatIdle standingCombatIdle;
    public MovingToCover movingToCover;
    public PeekingState peekingState;

    public AttackState(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;

        //combat states
        aimingState = new AimingState(stateManager);
        reloadingState = new ReloadingState(stateManager);
        shootingState = new ShootingState(stateManager);

        //movement states
        coverIdleState = new CoverIdleState(stateManager);
        aggroMovementState = new AggroMovementState(stateManager);
        standingCombatIdle  = new StandingCombatIdle(stateManager);
        movingToCover = new MovingToCover(stateManager);
        peekingState = new PeekingState(stateManager);












    }



    public override void OnEnter()
    {
        manager.attacking = true;
        currentCombatState = aimingState;
        currentMovementState = standingCombatIdle;
        currentSubState = aimingState;
        currentSubState.OnEnter();
       // currentCoverState = coverState;
    }

    public override State RunCurrentState()
    {
        Debug.Log("The current substate is: " + currentSubState);
        State nextSubState = currentSubState?.RunCurrentState();

        State nextMovementState = currentMovementState?.RunCurrentState();
        State nextCombatState = currentCombatState?.RunCurrentState();
        Debug.Log("The next substate is: " + nextSubState);



        if (nextSubState != null && nextSubState != currentSubState)
        {

            SwitchSubStates(nextSubState);
        }
        //else if (manager.isAlerted == false)
        //{
        //    currentSubState.OnExit();

        //    return manager.patrolState;




        //}

        else if (manager.onSearch == true)
        {

            currentSubState.OnExit();

            return manager.searchState;

        }

        //else if (nextSubState == null)
        //{
        //    Debug.Log("The next substate is null");
        //  //  currentSubState = null;
        //    return null;
            
        //}


            return this;

        
    }



    private void SwitchSubStates(State nextSubState) {

        currentSubState.OnExit();

        currentSubState = nextSubState;

        currentSubState.OnEnter();
    
    
    }



    public override void OnExit() {

        Debug.Log("Exiting attack");
        currentSubState.OnExit();
        currentSubState = null;
        manager.attacking = false;
    
    
    }
}

