using UnityEngine;

public class AttackState : State
{
    public StateManager manager;
   public State currentSubState;
    public State currentCoverState;
    public AimingState aimingState;
    //public CoverState coverState;
    public ReloadingState reloadingState;
    public ShootingState shootingState;
    

    public AttackState(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
        aimingState = new AimingState(stateManager);
      //  coverState = new CoverState(stateManager);
        reloadingState = new ReloadingState(stateManager);
        shootingState = new ShootingState(stateManager);


        
    }



    public override void OnEnter()
    {
        manager.attacking = true;
        currentSubState = aimingState;
        currentSubState.OnEnter();
       // currentCoverState = coverState;
    }

    public override State RunCurrentState()
    {
        Debug.Log("The current substate is: " + currentSubState);
        State nextSubState = currentSubState?.RunCurrentState();


      

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
