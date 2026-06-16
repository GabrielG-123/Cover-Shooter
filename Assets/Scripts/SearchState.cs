using UnityEditor.Analytics;
using UnityEngine;

public class SearchState : State
{
    StateManager manager;
    Vector3 LKP;  //last known position of player after losing sight of player
                  // bool hasLKP = false;
    Vector3 searchPoint;
    float searchTimer;
    float pathTimer;
    bool onSearch;
    public SearchState(StateManager stateManager) : base(stateManager)
    {

        manager = stateManager;

    }


    public override void OnEnter()
    {
        manager.animator.SetLayerWeight(1, 0f);
        LKP = manager.player.transform.position;
        manager.agent.destination = LKP;
        Debug.Log("turning on the bool");
        manager.animator.SetBool("isChasing", true);
        onSearch = manager.onSearch;
        searchTimer = 0;



        if (manager.coverScript.inCover) {


            if (manager.coverScript.checkExitCoverConditions())
            {


                manager.coverScript.ExitCover();


            }


        }





    }

    public override State RunCurrentState()
    {
        if (manager.animator.GetBool("isChasing")) 
        {

            manager.agent.speed = 5.0f;
           // manager.animator.speed = 1.85f;


        }

        else
        {
            manager.agent.speed = 2.5f;
            
        }


        if (manager.attacking || manager.alertMeter == 100)
        {

            return manager.attackState;



        }




        if (manager.agent.remainingDistance <= 0.3f && !manager.agent.pathPending && !onSearch) {
          //  Debug.Log("I am currently searching for the player");

            manager.onSearch = true;
            manager.agent.ResetPath();




        }

        if (searchTimer < 8.0f && onSearch)
        {
            //Debug.Log("The search timer is: " + searchTimer);

            searchTimer += Time.deltaTime;
            //generate a new point to go to only when enemy is near the previous point
            if (manager.agent.remainingDistance <= 0.3f || manager.inLOS && !manager.agent.pathPending)

            {
                manager.animator.SetBool("isChasing", false);
                manager.animator.SetBool("isSearching", false);
                manager.animator.SetBool("isSearchingPause", true);
                pathTimer += Time.deltaTime;
                searchPoint = Random.insideUnitSphere * 5f + LKP; 
                if (pathTimer > 5.0f)
                {
                    manager.animator.SetBool("isSearching", true);
                    manager.animator.SetBool("isSearchingPause", false);
                    manager.agent.destination = searchPoint;
                    pathTimer = 0;
                }

            }



        }
        else if(searchTimer > 8.0f){
            manager.onSearch = false;
            manager.attacking = false;
            return manager.patrolState;         //returning null because AttackState should not run a non-substate (E.g. PatrolState)
        
        
        }





            return this;

    }

    public override void OnExit()
    {
        Debug.Log("Running this function");
        manager.onSearch = false;
        manager.animator.SetBool("isChasing", false);
        manager.animator.SetBool("isSearching", false);
    }
}
