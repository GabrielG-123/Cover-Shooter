using UnityEngine;
using UnityEngine.AI;

public class AggroMovementState : State
{
    public StateManager manager;
    public State currentSubState;
    private bool hasPreviousPath = false;
    public bool arrivedAtAggroPoint;
    private Vector3 aggroPoint;
    public float aggroTimer;
    public float aggroDuration;
    public AggroMovementState(StateManager stateManager) : base(stateManager)
    {
        manager = stateManager;
    }

    public override State RunCurrentState()
    {



        return this;
    }

    public void AggroMovement()
    {


        // Only evaluate entering aggro if we aren't already locked into a path.
        // If we have a path, we are committed to finishing it.
        if (!hasPreviousPath)
        {

            bool inRange = manager.distanceFromPlayer >= 5.0f && manager.distanceFromPlayer <= 15.0f;
            manager.coverScript.isAggro = inRange && !manager.coverScript.CheckCoverConditions();
        }

        // Exit early if we are not aggro, or if a high-priority defensive override happens
        if (!manager.coverScript.isAggro || manager.coverScript.inCover || manager.coverScript.foundCover)
        {

            return;
        }

        Debug.Log("Still here");

        // 1. If we don't have a destination yet, find one
        if (!hasPreviousPath)
        {
            Debug.Log("Here we are");
            SetNewAggroDestination();

        }
        else
        {
            // 2. Check if the  has physically arrived at the point
            if (!manager.agent.pathPending && manager.agent.remainingDistance <= manager.agent.stoppingDistance)
            {
                if (!arrivedAtAggroPoint)
                {
                    // Just arrived this exact frame
                    Debug.Log("On this frame");
                    arrivedAtAggroPoint = true;
                    aggroDuration = Random.Range(1.0f, 3.0f);

                }
                else
                {
                    // 3. We are waiting at the point. Run the timer.
                    Debug.Log("Currently here");
                    aggroTimer += Time.deltaTime;

                    if (aggroTimer >= aggroDuration)
                    {
                        aggroTimer = 0f;
                        // Time is up. Now we are allowed to re-evaluate conditions.
                        bool inRange = manager.distanceFromPlayer >= 10.0f && manager.distanceFromPlayer <= 20.0f;
                        if (manager.coverScript.CheckCoverConditions())
                        {
                            manager.coverScript.isAggro = false;
                            hasPreviousPath = false; // Reset for next time we enter aggro
                        }
                        else
                        {
                            // Still in aggro, pick a new point and repeat
                            SetNewAggroDestination();
                        }
                    }
                }
            }
        }

        manager.animator.SetBool("strafing", true);
    }


    private void SetNewAggroDestination()
    {
        Debug.Log("Setting new aggro destination");
        Vector2 randomCircle = Random.insideUnitCircle * 6.5f;
        aggroPoint = new Vector3(randomCircle.x, 0, randomCircle.y) + manager.player.transform.position;

        if (NavMesh.SamplePosition(aggroPoint, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
        {
            manager.agent.SetDestination(navHit.position);
            hasPreviousPath = true;
            arrivedAtAggroPoint = false;
            // aggroTimer = 0f;
        }
    }



}
