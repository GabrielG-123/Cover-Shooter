using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class CoverScript : MonoBehaviour
{
    [SerializeField] StateManager manager;
    [SerializeField] CoverOccupation coverOccupation;
    [SerializeField] Suppression suppressionScript;
    public LayerMask searchLayer;
    public Collider hitboxCollider;
    int currentwaypointindex = 0;
    [SerializeField] private float closestColliderDistance = Mathf.Infinity;
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
    private Vector3 leftSideDistance;
    private Vector3 rightSideDistance;
    private Quaternion coverRotation;
    private Collider closestCover = null;
    public bool foundCover = false;
    public bool inCover = false;
    public bool exitingCover;
    public bool enteringCover;
    public bool canSeePlayer;
    public bool coverShooting = false;
    public bool coverPeek;
    public bool startCoverTimer;
    public bool canTakeCover = true;
    public bool alreadyTookCover = false;
    public bool isAggro;
    public bool isDefensive;
    public bool coverSpaces;
    public bool findNewSpace;
    public float playerAngle;
    public float coverAngle;
    public float coverTimer;
    public float backToCover;
    public float reenterCoverTimer;

    [SerializeField] float healthBeforeCover;
    [SerializeField] bool arrivedAtAggroPoint;
    [SerializeField] float aggroTimer;
    [SerializeField] float aggroDuration;


    private Collider[] occupiers = new Collider[100];

    public NavMeshHit navHit;

    public float rotationTimer;
    float timeToTurn = 0.5f;

    private void Awake()
    {
        manager = GetComponent<StateManager>();
    }

    private void Start()
    {
        healthBeforeCover = manager.healthScript.health;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(navHit.position, 0.5f);
    }

    private void Update()
    {
        arrivedAtAggroPoint = manager.attackState.shootingState.arrivedAtAggroPoint;
        aggroTimer = manager.attackState.shootingState.aggroTimer;
        aggroDuration = manager.attackState.shootingState.aggroDuration;

        if (Physics.Raycast(manager.head.position, manager.direction, out hit, 15))
        {
            if (hit.transform.CompareTag("Player"))
            {
                canSeePlayer = true;
            }
            else
            {
                canSeePlayer = false;
            }
        }

        Debug.DrawRay(manager.transform.position + new Vector3(0, 1.5f, 0), manager.direction * 15f, Color.red);

        if (closestCover != null)
        {
            directionToPlayer = manager.player.transform.position - closestCover.bounds.center;
            playerAngle = Vector3.Dot(closestCover.transform.right, directionToPlayer.normalized);
            coverAngle = Vector3.Dot(oppositeNormal, closestCover.transform.forward);
        }

        if (startCoverTimer)
        {
            reenterCoverTimer += Time.deltaTime;
            if (reenterCoverTimer >= 5.0f)
            {
                canTakeCover = true;
                startCoverTimer = false;
                reenterCoverTimer = 0;
            }
        }

        FindCoverSpace();
    }

    public bool CheckCoverConditions()
    {
        if (manager.healthScript.health <= (healthBeforeCover * 0.5f) && canTakeCover)
        {
            isDefensive = true;
            return true;
        }
        if (manager.myGun.ammoInClip <= (manager.myGun.ammoInClipMax * 0.2f) && canTakeCover)
        {
            Debug.Log("Currently here too");
            isDefensive = true;
            return true;
        }
        isDefensive = false;
        return false;
    }

    public bool CheckExitCoverConditions()
    {
        // If the agent is on search, or they failed to find any valid cover, force exit state
        if (manager.onSearch || !canTakeCover)
        {
            return true;
        }
        return false;
    }

    private void FindCoverSpace()
    {
        // Do not constantly search for new space if the agent is already comfortably in cover
        if (!foundCover || closestCover == null || inCover)
        {
            return;
        }

        Debug.Log("Finding cover space for agent: " + manager.transform.name);

        int occupierCount = Physics.OverlapSphereNonAlloc(navHit.position, 0.5f, occupiers, searchLayer);
        findNewSpace = false;

        for (int i = 0; i < occupierCount; i++)
        {
            if (occupiers[i] != hitboxCollider && occupiers[i].CompareTag("Enemy"))
            {
                findNewSpace = true;
                break;
            }
        }

        Debug.Log("The value of findNewSpace is: " + findNewSpace);

        if (!findNewSpace)
        {
            return;
        }

        Vector3 coverOrientation = navHit.position - closestCover.bounds.center;
        Debug.DrawRay(closestCover.bounds.center, coverOrientation.normalized * 15.0f, Color.blue);

        float dotZ = Mathf.Abs(Vector3.Dot(coverOrientation, closestCover.transform.forward));
        float dotX = Mathf.Abs(Vector3.Dot(coverOrientation, closestCover.transform.right));

        float ratioX = dotX / closestCover.bounds.extents.x;
        float ratioZ = dotZ / closestCover.bounds.extents.z;

        Vector3 coverFace;
        float faceExtent = 0f;

        if (ratioX > ratioZ)
        {
            coverFace = closestCover.transform.forward;
            faceExtent = closestCover.bounds.extents.z;
        }
        else
        {
            coverFace = closestCover.transform.right;
            faceExtent = closestCover.bounds.extents.x;
        }

        float coverSpacing = 1.5f;
        int maxSpaceIntervals = Mathf.FloorToInt((faceExtent * 2) / coverSpacing);

        if (coverOccupation != null && coverOccupation.coverOccupiers >= maxSpaceIntervals)
        {
            coverOccupation.isOccupied = true;
            closestCover = null;
            foundCover = false;
            FindCover();
            return;
        }

        for (int i = 1; i <= maxSpaceIntervals; i++)
        {
            Debug.Log("The value of maxSpaces: " + maxSpaceIntervals);
            Vector3 positivePos = navHit.position + (coverFace * (i * coverSpacing));
            float posDistFromCenter = Mathf.Abs(Vector3.Dot(positivePos - closestCover.bounds.center, coverFace));

            if (posDistFromCenter <= faceExtent)
            {
                if (!Physics.CheckSphere(positivePos, 0.4f, searchLayer))
                {
                    manager.agent.destination = positivePos;
                    navHit.position = positivePos;
                    return;
                }
            }

            Vector3 negativePos = navHit.position + (-coverFace * (i * coverSpacing));
            float negDistFromCenter = Mathf.Abs(Vector3.Dot(negativePos - closestCover.bounds.center, coverFace));

            if (negDistFromCenter <= faceExtent)
            {
                if (!Physics.CheckSphere(negativePos, 0.4f, searchLayer))
                {
                    manager.agent.destination = negativePos;
                    navHit.position = negativePos;
                    return;
                }
            }
        }

        if (coverOccupation != null)
        {
            coverOccupation.isOccupied = true;
        }
        closestCover = null;
        foundCover = false;
        FindCover();
    }

    public void FindCover()
    {
        Debug.Log("Finding cover for agent: " + manager.transform.name);


        //closestColliderDistance = Mathf.Infinity;
        Debug.Log("The new value of closestColliderDistance is: " + closestColliderDistance);
        closestCover = null;
        coverSpaces = true;
        coverOccupation = null;

        manager.animator.SetBool("strafing", true);

        // Increased search radius to 100.0f to guarantee the agent finds the next closest cover
        Collider[] hitColliders = Physics.OverlapSphere(manager.transform.position, 10.0f);


        foreach (var hitCollider in hitColliders)
        {

            if (hitCollider.CompareTag("Cover"))
            {
                Debug.Log("The cover object found is: " + hitCollider.name);
                distanceToCover = hitCollider.transform.position - manager.transform.position;
                distMagnitude = distanceToCover.sqrMagnitude;

                if (distMagnitude < closestColliderDistance)
                {
                    CoverOccupation occ = hitCollider.GetComponent<CoverOccupation>();

                    if (occ != null && !occ.isOccupied)
                    {
                        closestColliderDistance = distMagnitude;
                        coverOccupation = occ;
                        closestCover = hitCollider;
                    }
                }
            }
        }

        if (closestCover != null)
        {
            Debug.Log("The closest cover is: " + closestCover.name + "and the agent's name is: " + manager.transform.name);

            coverOffset = closestCover.bounds.extents.magnitude * 0.5f;
            directionToCover = closestCover.transform.position - manager.player.transform.position;
            coverCenter = closestCover.bounds.center;
            coverPoint = coverCenter + (directionToCover.normalized * coverOffset);
            closestPoint = closestCover.ClosestPoint(coverPoint);
            leftSideDistance = coverCenter - closestCover.bounds.extents.x * closestCover.transform.right;
            rightSideDistance = coverCenter + closestCover.bounds.extents.x * closestCover.transform.right;

            // Increased sample radius to 4.0f to prevent NavMesh failure on thicker cover objects
            if (NavMesh.SamplePosition(closestPoint, out navHit, 4.0f, NavMesh.AllAreas))
            {
                manager.agent.destination = navHit.position;
                foundCover = true;
            }
        }
        else
        {
            // If absolutely no available cover exists in the radius, force the agent out of the cover state so it doesn't freeze
            canTakeCover = false;
            startCoverTimer = true;

            // Clear the path so the agent actually stops trying to take the occupied spot
            if (manager.agent.isOnNavMesh)
            {
                manager.agent.ResetPath();
            }
        }
    }

    public void EnterCover()
    {
        closestColliderDistance = Mathf.Infinity;
        healthBeforeCover = manager.healthScript.health;

        if (coverOccupation != null)
        {
            coverOccupation.coverOccupiers += 1;
        }

        if (manager.myGun.ammoInClip <= (manager.myGun.ammoInClipMax * 0.2f))
        {
            manager.myGun.reload(1);
        }

        foundCover = false;
        Debug.Log("yes, here");
        enteringCover = true;
        manager.RigLayers[0].active = false;

        manager.agent.enabled = false;

        Sequence coverSequence = DOTween.Sequence();
        coverSequence.Append(manager.agent.transform.DOMove(navHit.position, 0.5f));
        coverSequence.Append(manager.agent.transform.DORotateQuaternion(coverRotation, 0.5f));
        coverSequence.OnComplete(() =>
        {
            manager.agent.enabled = true;
        });

        if (manager.animator.GetBool("isReloading"))
        {
            manager.animator.SetLayerWeight(1, 1f);
        }
        else
        {
            manager.animator.SetLayerWeight(1, 0f);
        }

        manager.animator.SetBool("isShooting", false);
        manager.animator.SetBool("strafing", false);

        if (playerAngle > 0f && (coverAngle <= -0.99f))
        {
            manager.animator.SetBool("shouldMirror", true);
        }

        if (playerAngle < 0f && coverAngle >= 0.99f)
        {
            manager.animator.SetBool("shouldMirror", true);
        }
    }

    public void ExitCover()
    {
        isDefensive = false;
        Debug.Log("We are in this function");

        if (!alreadyTookCover)
        {
            alreadyTookCover = true;
        }
        Debug.Log("This should run");

        if (inCover)
        {
            coverShooting = false;
            coverPeek = false;
            Debug.Log("We are in cover, trying to exit");

            if (!coverPeek)
            {
                Debug.Log("We are not peeking, exiting cover");
                inCover = false;
                Debug.Log("inCover is: " + inCover); ;
                manager.animator.SetBool("takeCover", false);
                Debug.Log("status of takeCover is: " + manager.animator.GetBool("takeCover"));
                manager.animator.SetBool("exitCover", true);
                manager.RigLayers[0].active = false;
                manager.animator.SetLayerWeight(1, 0f);
                Debug.Log("We are here");

                startCoverTimer = true;
                canTakeCover = false;
            }
        }
    }

    public void FinishCoverAnimation()
    {
        enteringCover = false;
    }

    public void ExitCoverAnimation()
    {
        if (!inCover)
        {
            manager.animator.SetBool("exitCover", false);
        }
    }

    public void MovingToCover()
    {
        Debug.Log("This is now running for agent: " + manager.transform.name);
        rotationTimer += Time.deltaTime;

        if (inCover == true)
        {
            manager.agent.updateRotation = false;
        }
        else
        {
            manager.agent.transform.rotation = Quaternion.Slerp(manager.agent.transform.rotation, manager.targetRotation, rotationTimer / timeToTurn);
        }

        if (inCover == true && enteringCover == false)
        {
            manager.agent.updateRotation = true;
        }

        Debug.DrawRay(manager.player.transform.position, directionToCover * 5.0f, Color.green);

        if (manager.agent.remainingDistance <= manager.agent.stoppingDistance && !manager.agent.pathPending)
        {
            Debug.Log("This is currently running for agent: " + manager.transform.name);
            if (closestCover == null) return;
            distanceToCover = closestCover.transform.position - manager.transform.position;

            if (Physics.Raycast(manager.agent.transform.position, distanceToCover, out coverHit, 5.0f))
            {
                oppositeNormal = -coverHit.normal;
                coverRotation = Quaternion.LookRotation(oppositeNormal, Vector3.up);
                Debug.Log("Doing this currently");
                manager.animator.SetBool("takeCover", true);
                Debug.Log("Yup, true");
                inCover = true;
            }
        }
        else
        {
            Debug.Log("This is now running for the agent: " + manager.transform.name);
        }
    }

    public void coverShootCheck()
    {
        Debug.DrawRay(manager.player.transform.position + new Vector3(0, 1.0f, 0), -manager.direction * 20f, Color.green);
        Debug.Log("Still running this function");

        if (inCover && !coverPeek)
        {
            if (Physics.Raycast(manager.player.transform.position + new Vector3(0, 1.0f, 0), -manager.direction, out RaycastHit localHit, 20f))
            {
                if (localHit.transform.CompareTag("Cover"))
                {
                    Debug.Log("Yes");
                    coverShooting = true;
                }
                else if (localHit.transform.CompareTag("Enemy"))
                {
                    Debug.Log("No, I am hitting: " + localHit.collider.name);
                    coverShooting = false;
                    manager.RigLayers[0].active = true;
                    Debug.Log("This is happening");
                    manager.animator.SetLayerWeight(1, 1f);
                    manager.animator.SetBool("isShooting", true);
                }
            }
        }

        if (coverShooting)
        {
            coverTimer += Time.deltaTime;

            

            if (suppressionScript.suppressionAmount >= (suppressionScript.maxSuppression * 0.5f))
            {
                Debug.Log("Applying suppression , cover timer is: " + coverTimer);
                if (coverTimer >= Random.Range(5, 8))
                {
                    Debug.Log("Turning on the animations");
                    manager.animator.SetBool("isPeeking", true);
                    manager.animator.SetBool("isShooting", true);
                    manager.animator.SetLayerWeight(1, 1f);
                    manager.RigLayers[0].active = true;
                    coverPeek = true;
                    Debug.Log("Turning cover peek on");



                }




            }

            else {

                if (coverTimer >= Random.Range(1, 4))
                {
                    Debug.Log("We are doing this now");
                    manager.animator.SetBool("isPeeking", true);
                    manager.animator.SetBool("isShooting", true);
                    manager.animator.SetLayerWeight(1, 1f);
                    manager.RigLayers[0].active = true;
                    coverPeek = true;
                }

            }

            if (coverPeek)
            {
                backToCover += Time.deltaTime;

                if (backToCover >= Random.Range(3, 6))
                {
                    manager.animator.SetBool("isPeeking", false);
                    manager.animator.SetBool("isShooting", false);
                    manager.animator.SetLayerWeight(1, 0f);
                    manager.RigLayers[0].active = false;
                    coverPeek = false;

                    coverTimer = 0;
                    backToCover = 0;
                }
                else if (manager.animator.GetBool("isReloading"))
                {
                    manager.animator.SetBool("isPeeking", false);
                    manager.animator.SetBool("isShooting", false);
                    coverPeek = false;
                    coverTimer = 0;
                    backToCover = 0;
                }

                if (suppressionScript.suppressionAmount >= (suppressionScript.maxSuppression * 0.5f))
                {

                    if (backToCover >= Random.Range(1, 3))
                    {


                        manager.animator.SetBool("isPeeking", false);
                        manager.animator.SetBool("isShooting", false);
                        manager.animator.SetLayerWeight(1, 0f);
                        manager.RigLayers[0].active = false;
                        coverPeek = false;
                        Debug.Log("Turning cover peek off");
                        coverTimer = 0;
                        backToCover = 0;


                    }



                }

            }


            

        }
    }
}