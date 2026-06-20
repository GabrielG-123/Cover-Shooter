using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class StateManager : MonoBehaviour
{

    public NavMeshAgent agent;
    private State currentState;
    public Animator animator;
    public ObjectHealth healthScript;
    public CoverState coverScript;
    public bool isAlerted;
    public GameObject player;
    public Transform[] waypoints;
    public Transform head;
    public Vector3 direction;
    public Vector3 localVelocity;
    public Quaternion targetRotation;
    public float coneAngle;
    public float distanceFromPlayer;
    public bool inLOS;
    public bool onSearch;
    public bool attacking;
    public bool firing;
    public bool enteringCover;
    public bool takingCover;
    public float alertMeter = 0;
    public float burstShotsFired = 0;
    public float xSpeed;
    public float zSpeed;

    public Transform testCover;




    public IdleState idleState;
    public PatrolState patrolState;
    public AttackState attackState;
    public SearchState searchState;


    public Gun myGun;
    public RigBuilder RigBuilder;
    public List<RigLayer> RigLayers;

    public int MoveXHash;
    public int MoveZHash;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        idleState = new IdleState(this);
        patrolState = new PatrolState(this);
        attackState = new AttackState(this);
        searchState = new SearchState(this);



        


        currentState = idleState;


        RigLayers = RigBuilder.layers;




    }

    private void Awake()
    {
        healthScript = GetComponent<ObjectHealth>();

        MoveXHash = Animator.StringToHash("MoveX");
        MoveZHash = Animator.StringToHash("MoveZ");
    }


    private void OnEnable()
    {
        healthScript.OnTakeDamage += HealthScript_OnTakeDamage;
        healthScript.OnDeath += HealthScript_OnDeath;
    }


    private void OnDisable()
    {
        healthScript.OnTakeDamage -= HealthScript_OnTakeDamage;
        healthScript.OnDeath -= HealthScript_OnDeath;
    }
    private void HealthScript_OnDeath()
    {
        Destroy(gameObject);
    }

    private void HealthScript_OnTakeDamage(float damage, GameObject damageDealer)
    {
        if (damageDealer != null && damageDealer.CompareTag("Player"))
        {

            attacking = true;



        }


    }
    private void OnDrawGizmos()
    {
      //  Gizmos.color = Color.yellow;
      //  Gizmos.DrawSphere(attackState.coverState.coverPoint, 0.3f);
      //  Gizmos.DrawSphere(attackState.coverState.navHit.position, 0.3f);
      //  Gizmos.color = Color.green;
      //  Gizmos.DrawSphere(attackState.coverState.coverCenter, 0.3f);
      //  Gizmos.color = Color.blue;
      //  Gizmos.DrawSphere(attackState.coverState.coverHit.point, 0.3f);
    }



    // Update is called once per frame
    void Update()
    {
        RunStateMachine();

        enteringCover = coverScript.enteringCover;


        //always updating the direction vector between enemy and player
        //simultaneously updating the proper rotation for when enemy needs to turn to player
        direction = player.transform.position - transform.position;
        targetRotation = Quaternion.LookRotation(direction);
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);


        //find the cosine of the angle between enemy's forward vector and direction vector from enemy to player. 
        coneAngle = Vector3.Dot(direction.normalized, head.forward);
        




        localVelocity = transform.InverseTransformDirection(agent.velocity);






        //   agent.speed = (animator.deltaPosition / Time.deltaTime).magnitude;
        // Debug.Log("The agent's velocity is: " + agent.velocity);
        // Debug.Log("The agent's speed is: " + agent.speed);
        //Debug.Log("Magnitude of agent's velocity is: " + Vector3.Magnitude(agent.velocity));

        xSpeed = localVelocity.x / agent.speed;
        zSpeed = localVelocity.z / agent.speed;


        //if (coverScript.startCoverTimer)
        //{

        //    coverScript.reenterCoverTimer += Time.deltaTime;
        //    if (coverScript.reenterCoverTimer >= 5.0f)
        //    {

        //        coverScript.startCoverTimer = false;
        //        coverScript.reenterCoverTimer = 0;
        //        coverScript.canTakeCover = true;



        //    }


        //}
        //else {

        //    if (coverScript.alreadyTookCover)
        //    {
        //        coverScript.canTakeCover = false;


        //    }
        
        //}

      



        //  Debug.Log("The agent's speed is: " + agent.speed);


        // Debug.
        // (transform.position + new Vector3(0, 1, 0), direction * 10f, Color.yellow);
       // Debug.
       // (transform.position, direction * 10f, Color.blue);
       // Debug.DrawRay(head.position, head.forward * 10f, Color.green);





    }
    
    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState();




        if (nextState != null && nextState != currentState)
        {

            //  Debug.Log("Switching states..");
            SwitchToNextState(nextState);

        }



        else if (nextState == null) {

            Debug.Log("Next state is null");

        }
        inLOS = currentState.visionCone();

        Debug.Log("The current state running is: " + currentState);
    }


    private void SwitchToNextState(State NextState) {

        //perform cleanup on outgoing state first
        currentState.OnExit();

        currentState = NextState;

        currentState.OnEnter();





    }


    //public void RunCoverMethod() {

    //    if (currentState == attackState) {

    //        if (attackState.currentSubState == attackState.coverState) {

    //            attackState.coverState.EnterCover();
            
            
            
    //        }
        
    //    }
    
    
    
    //}


    //public void startShooting() { 
    
            
    
    
    
    //}
}