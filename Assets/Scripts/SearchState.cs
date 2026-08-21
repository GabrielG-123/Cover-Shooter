using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : State
{
    StateManager manager;
    Vector3 LKP;  //last known position of player after losing sight of player
                  // bool hasLKP = false;
    Vector3 searchPoint;
    float searchTimer;
    float pathTimer;
    bool onSearch;
    bool isAtLKP = false;
    bool startRadiusSearch = false;
    public SearchState(StateManager stateManager) : base(stateManager)
    {

        manager = stateManager;

    }


    public override void OnEnter()
    {
        if (!manager.agent.isActiveAndEnabled)
        {
            manager.agent.enabled = true;
        }

        if (!manager.agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(manager.transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                //use warp to instantly snap agent to a valid position on the NavMesh
                manager.agent.Warp(hit.position);
            }
        }

        if (manager.agent.isOnNavMesh)
        {
            manager.agent.ResetPath();
        }
       
        manager.animator.SetLayerWeight(1, 0f);
        LKP = manager.player.transform.position;

        // Validate LKP is on the NavMesh before using it
        if (NavMesh.SamplePosition(LKP, out NavMeshHit lkpHit, 2.0f, NavMesh.AllAreas))
        {
            //check first if the agent is on the NavMesh before setting the destination
            if (manager.agent.isOnNavMesh)
            {
                manager.agent.destination = lkpHit.position;
            }
            LKP = lkpHit.position;

            Debug.Log("turning on the bool");
            Debug.Log("The last known position of the player is: " + LKP + " and the destination of the agent is: " + manager.agent.destination);
        }
        else
        {
            Debug.LogWarning("LKP is off NavMesh, agent cannot navigate there: " + LKP);
            manager.agent.destination = manager.transform.position; // fallback to current position
        }

        manager.animator.SetBool("isChasing", true);
        onSearch = manager.onSearch;
        searchTimer = 0;



        if (manager.coverScript.inCover) {


            if (manager.coverScript.CheckExitCoverConditions())
            {


                manager.coverScript.ExitCover();

                manager.coverScript.inCover = false;
                

                // manager.animator.SetBool("exitCover", false);



            }


        }





    }

    public override State RunCurrentState()
    {


        Debug.Log("is at LKP is: " + isAtLKP);
        if (manager.animator.GetBool("isChasing"))
        {

            manager.agent.speed = 4.0f;
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




        if (manager.agent.remainingDistance <= 0.3f || !manager.agent.pathPending && !onSearch) {
            Debug.Log("I am currently searching for the player and my name is:" + manager.transform.name );

            manager.onSearch = true;
            manager.agent.ResetPath();




        }

        if (Vector3.Distance(manager.agent.transform.position, LKP) <= 0.5f && !manager.agent.pathPending) {
            Debug.Log("We have arrived at LKP");
            isAtLKP = true;




        }

        if (isAtLKP && !manager.inLOS) {

            startRadiusSearch = true;
        
        
        }

        if (startRadiusSearch)
        {
            if (searchTimer < 20.0f && onSearch)
            {
                //Debug.Log("The search timer is: " + searchTimer);

                searchTimer += Time.deltaTime;
                //generate a new point to go to only when enemy is near the previous point


                if (manager.agent.remainingDistance <= 0.3f && !manager.inLOS && !manager.agent.pathPending)

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





                if (searchTimer > 20.0f)
                {
                    manager.onSearch = false;
                    manager.attacking = false;
                    startRadiusSearch = false;
                    manager.isAlerted = false;
                    Debug.Log("Moving to patrol state");
                    return manager.patrolState;         //returning null because AttackState should not run a non-substate (E.g. PatrolState)


                }


            }
        }


        


            return this;

    }

    public override void OnExit()
    {
      //  Debug.Log("Running this function");
        manager.onSearch = false;
        manager.animator.SetBool("isChasing", false);
        manager.animator.SetBool("isSearching", false);
    }
}
