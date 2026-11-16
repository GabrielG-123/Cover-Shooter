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
        currentMovementState = aggroMovementState;
       // currentSubState = aimingState;
       // currentSubState.OnEnter();
        currentMovementState.OnEnter();
        currentCombatState.OnEnter();
        
       // currentCoverState = coverState;
    }

    public override State RunCurrentState()
    {
        Debug.Log("The current movementstate is: " + currentMovementState);
        Debug.Log("The current combatState is: " + currentCombatState);
      //  State nextSubState = currentSubState?.RunCurrentState();

        State nextMovementState = currentMovementState?.RunCurrentState();
        State nextCombatState = currentCombatState?.RunCurrentState();
        //Debug.Log("The next substate is: " + nextSubState);



        //if (nextSubState != null && nextSubState != currentSubState)
        //{

        //    SwitchSubStates(nextSubState);
        //}

        if (nextMovementState != null && nextMovementState != currentMovementState) {

            SwitchMovementStates(nextMovementState);
            


        }

        if (nextCombatState != null && nextCombatState != currentCombatState) {



            SwitchCombatStates(nextCombatState);
        
        }
        //else if (manager.isAlerted == false)
        //{
        //    currentSubState.OnExit();

        //    return manager.patrolState;




        //}

        else if (manager.onSearch == true)
        {

            currentCombatState.OnExit();
            currentMovementState.OnExit();

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

    private void SwitchMovementStates(State nextMovementState) {
        currentMovementState.OnExit();
        currentMovementState = nextMovementState;
        currentMovementState.OnEnter();
    
    }

    private void SwitchCombatStates(State nextCombatState) {


        currentCombatState.OnExit();
        currentCombatState = nextCombatState;
        currentCombatState.OnEnter();
    
    
    }


    public override void OnExit() {

        Debug.Log("Exiting attack");
        currentCombatState.OnExit();
        currentMovementState.OnExit();

        currentCombatState = null;
        currentMovementState = null;
        
        manager.attacking = false;
    
    
    }
}

