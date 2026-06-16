using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Animations;
using DG.Tweening;
using UnityEngine.TestTools;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Cinemachine.Editor;

public class CoverState : MonoBehaviour
{
    [SerializeField] StateManager manager;
    int currentwaypointindex = 0;
    private float closestColliderDistance = Mathf.Infinity;
    private float currentColliderDistance;
    private float coverOffset;
    private float distMagnitude;
    private Vector3 directionToCover;
    private Vector3 directionToPlayer;
    public RaycastHit coverHit;
    public RaycastHit hit;
    public Vector3 coverPoint;
    public Vector3 closestPoint;
    public Vector3 coverCenter;
    private Vector3 distanceToCover;
    private Vector3 oppositeNormal;
    private Quaternion coverRotation;
    private Collider closestCover = null;
    public bool foundCover = false;
    public bool inCover = false;
    public bool exitingCover;
    public bool enteringCover;
    public bool canSeePlayer;
    public bool coverShooting = false;
    public bool coverPeek;
    public bool startCoverTimer; // this is used to start a cooldown timer after the agent leaves cover before they can take cover again.
    public bool canTakeCover = true;
    public float playerAngle;
    public float coverAngle;
    public float coverTimer;
    public float backToCover;
    public float reenterCoverTimer;

    public NavMeshHit navHit;

    public float rotationTimer;
    float timeToTurn = 0.5f;





    private void Awake()
    {
                manager = GetComponent<StateManager>();
    }


    private void Update()
    {
       // Debug.Log("Entering cover: " + enteringCover);
       // Debug.Log("The agent is in cover: " + inCover);
       // Debug.Log("The agent's rotation is being updated" + manager.agent.updateRotation);


        if (Physics.Raycast(manager.head.position, manager.direction, out hit, 15)) {


            if (hit.transform.CompareTag("Player")) {

                
             //   Debug.Log("Hitting player");
                    canSeePlayer = true;
            
            }

            else
            {
               // Debug.Log("The collider I am hitting is: " + hit.transform.name);
                //Debug.Log("not hitting player");
                canSeePlayer = false;
            }
        
        
        }

       Debug.DrawRay(manager.transform.position + new Vector3(0, 1.5f, 0), manager.direction * 15f, Color.red);


        if (closestCover != null) directionToPlayer = manager.player.transform.position - closestCover.bounds.center;


        //  Debug.DrawRay(closestCover.bounds.center, directionToPlayer * 10f, Color.red);

        playerAngle = Vector3.Dot(closestCover.transform.right, directionToPlayer.normalized);
      //  Debug.Log("The closest cover is" + closestCover.name);

       // Debug.Log("The dot product between cover and negative normal is: " + Vector3.Dot(oppositeNormal, closestCover.transform.forward));

        coverAngle = Vector3.Dot(oppositeNormal, closestCover.transform.forward);


       
    }
    //public CoverState(StateManager stateManager) : base(stateManager)
    //{
    //    manager = stateManager;
    //}
    public bool checkCoverConditions() {

        if (manager.healthScript.health <= 50 && canTakeCover) {


            return true;
        
        }


        if (manager.myGun.ammoInClip <= (manager.myGun.ammoInClipMax * 0.2f) && canTakeCover) {

            return true;
        
        
        }

        return false;
    
    
    }


    public bool checkExitCoverConditions() {

        if (manager.onSearch) {


            return true;
        
        }
        


        
        
        
        
        
        
        
        
        
        return false;
    
    
    
    
    
    }
    public  void FindCover()
    {
       // Debug.Log("Entering the cover state");
        manager.animator.SetBool("strafing", true);
       // manager.agent.updateRotation = false;
        

        Collider[] hitColliders = Physics.OverlapSphere(manager.transform.position, 20.0f);



        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Cover"))
            {

                distanceToCover = hitCollider.transform.position - manager.transform.position;
                distMagnitude = distanceToCover.sqrMagnitude;
                // Debug.Log("This is running");


                if (distMagnitude < closestColliderDistance)
                {
                    closestColliderDistance = distMagnitude;
                    closestCover = hitCollider;



                }




               // Debug.DrawRay(manager.player.transform.position + new Vector3(0.1f, 0, 0), directionToCover.normalized * directionToCover.magnitude, Color.yellow);
             //   Debug.Log("The normalized vector is: " + directionToCover.normalized);
            }



        }

        if (closestCover != null)
        {
          //  Debug.Log("The closest cover is: " + closestCover.name);
            coverOffset = closestCover.bounds.extents.magnitude * 0.5f;
            directionToCover = closestCover.transform.position - manager.player.transform.position;
           // directionToCover.y = 0;
            coverCenter = closestCover.bounds.center;
            coverPoint = coverCenter + (directionToCover.normalized * coverOffset);
            closestPoint = closestCover.ClosestPoint(coverPoint);

            if (NavMesh.SamplePosition(closestPoint, out navHit, 1.5f, NavMesh.AllAreas)) {

                manager.agent.destination = navHit.position;




            }
            

           
           // Debug.Log("The status of the path is:" + manager.agent.pathStatus);



            foundCover = true;


        }

        
    }


    public void EnterCover() {

        


        enteringCover = true;
        manager.RigLayers[0].active = false;
        Sequence coverSequence = DOTween.Sequence(); //start DOTween timeline
       coverSequence.Append(manager.agent.transform.DOMove(coverHit.point, 0.5f));
       coverSequence.Append(manager.agent.transform.DORotateQuaternion(coverRotation, 0.5f));
       
       manager.animator.SetLayerWeight(1, 0f);

        manager.animator.SetBool("isShooting", false);
        if (playerAngle > 0f && (coverAngle <= -0.99f))
        {
            //Debug.Log("This is true");
            manager.animator.SetBool("shouldMirror", true);




        }
        else
        {
            //Debug.Log("this is not true");
        }

        if (playerAngle < 0f && coverAngle >= 0.99f)
        {


           // Debug.Log("This is true");
            manager.animator.SetBool("shouldMirror", true);


        }






    }



    public void ExitCover()
    {
        Debug.Log("This should run");

        if (inCover) {

            if (!coverPeek) {
                Debug.Log("This should happen");
                manager.animator.SetBool("exitCover", true);
                manager.RigLayers[0].active = false;
                manager.animator.SetLayerWeight(1, 0f);
                Debug.Log("We are here");
                inCover = false;
                startCoverTimer = true;
                canTakeCover = false;



            }
        
        
        
        }







    }

    public void finishCoverAnimation() {


        enteringCover = false;
       






    }


    
    public void MovingToCover()
    {

        rotationTimer += Time.deltaTime;


     //   Debug.Log("The agent is in cover: " + inCover);

        if (inCover == true)
        {

            manager.agent.updateRotation = false;

        }
        else {

            manager.agent.transform.rotation = Quaternion.Slerp(manager.agent.transform.rotation, manager.targetRotation, rotationTimer / timeToTurn);


        }

        if (inCover == true && enteringCover == false)
        {
            manager.agent.updateRotation = true;
            



        }

       
          //  Debug.Log("The closes point is: " + closestPoint);


      //  Debug.DrawRay(manager.agent.transform.position, distanceToCover * 5.0f, Color.red);
       // Debug.DrawRay(manager.player.transform.position, directionToCover * 5.0f, Color.green);
            
            manager.animator.SetFloat(manager.MoveXHash, Mathf.Clamp(manager.localVelocity.x, -1f, 1f));
        manager.animator.SetFloat(manager.MoveZHash, Mathf.Clamp(manager.localVelocity.z, -1f, 1f));


        if (manager.agent.remainingDistance == manager.agent.stoppingDistance && !manager.agent.pathPending) {
          //  Debug.Log("This is running!");
            distanceToCover = closestCover.transform.position - manager.transform.position;

            if (Physics.Raycast(manager.agent.transform.position, distanceToCover, out coverHit, 5.0f)) {

                oppositeNormal = -coverHit.normal;
                coverRotation = Quaternion.LookRotation(oppositeNormal, Vector3.up);

                manager.animator.SetBool("takeCover", true);


                

                inCover = true;

            
            
            
            }
            
        
        
        }

            

     }


    public void coverShootCheck() {

        // Debug.Log("The dot product is: " + Vector3.Dot(manager.transform.forward, manager.direction.normalized));

        Debug.DrawRay(manager.player.transform.position + new Vector3(0, 1.0f, 0), -manager.direction * 20f, Color.green);


        if (inCover && !coverPeek)
        {

            if (Physics.Raycast(manager.player.transform.position + new Vector3(0, 1.0f, 0), -manager.direction, out RaycastHit localHit, 20f)) {

                if (localHit.transform.CompareTag("Cover"))
                {
                    Debug.Log("Yes");

                    coverShooting = true;


                }


                else {

                    Debug.Log("No, I am hitting: " + localHit.collider.name);
                    coverShooting = false;
                    manager.RigLayers[0].active = true;
                    Debug.Log("This is happening");
                    manager.animator.SetLayerWeight(1, 1f);
                    manager.animator.SetBool("isShooting", true);


                }



                





            }

            

        }



        //if (coverShooting)
        //{

        //    coverTimer += Time.deltaTime;


        //    if (coverTimer >= Random.Range(3, 6))
        //    {
                
        //        manager.animator.SetBool("isPeeking", true);
        //        manager.animator.SetBool("isShooting", true);
        //        manager.animator.SetLayerWeight(1, 1f);
        //        manager.RigLayers[0].active = true;

        //        coverPeek = true;





        //    }


        //    if (coverPeek)
        //    {

        //        backToCover += Time.deltaTime;

        //        if (backToCover >= Random.Range(3, 6))
        //        {

        //            manager.animator.SetBool("isPeeking", false);
        //            manager.animator.SetBool("isShooting", false);
        //            manager.animator.SetLayerWeight(1, 0f);
        //            manager.RigLayers[0].active = false;


        //            coverPeek = false;

        //            coverTimer = 0;
        //            backToCover = 0;

        //        }


        //        else if (manager.animator.GetBool("isReloading"))
        //        {


        //            manager.animator.SetBool("isPeeking", false);
        //            manager.animator.SetBool("isShooting", false);
        //          //  manager.animator.SetLayerWeight(1, 0f);
        //           // manager.RigLayers[0].active = false;


        //            coverPeek = false;

        //            coverTimer = 0;
        //            backToCover = 0;


        //        }






        //    }




        //}




















    }
}
