using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    public GameObject bulletObject;
    public Transform bulletSpawn;
    public float bulletSpeed;
    public float bulletLifetime;
    [SerializeField] float bulletDamage;
    public float ammoInClip;
    public float ammoInClipMax;
    public float ammoReserve;
    public float fireRate = 10.1f;
    public float nextFire = 0.0f;
    private int bulletsFired;
    [SerializeField] Vector3 bulletSpreadRadius;
    [SerializeField] private Vector3 Spread;
    [SerializeField] private Vector3 centerPosPixels;
    [SerializeField] private Vector3 radiusPosPixels;
    private Vector2 maxCrosshairSpread;
    private float pixelDistance;
    [SerializeField] private float pixelDiameter;
    private float crosshairWidth;
    private float crosshairHeight;
    public float bulletSpreadFactor;
    public float bulletSpreadIncrement;
    public float decayRate;
    public float maxSpread;
    public float minSpread;
    public float shotsForSpread;
    public float timeToReset;
    public float timeElapsed;
    public float resetTracker;
    public float minimumShotsBeforeSpread;
   public bool addSpread = false;
    private bool targetInFront; //used to determine if a collider is in the same direction as the bullet raycast
    [SerializeField] private float resolutionFactor;
    private float hitboxMultiplier;
    private int layerIndex;

    //for AI only: 
    public float burstRounds;
    public float timeBetweenBursts;





    public TwoBoneIKConstraint leftHandIK;
    public MultiAimConstraint gunAimConstraint;
    public MultiAimConstraint RightShoulderAimConstraint;
    public MultiAimConstraint spineAim;
    float initialgunAimWeight;
    float initialRightShoulderWeight;
    float initialSpineAimWeight;


    // bool canShoot;
    bool isReloading;

    ObjectHealth target;
    HitboxDamage hitbox;
    [SerializeField] ParticleSystem muzzleflash;
    [SerializeField] WeaponRecoil recoil;

    public Transform gunRaycast;
    public GameObject bulletHole;
    private GameObject bulletClone;
    [SerializeField] RectTransform crosshairTransform;
    [SerializeField] Canvas CrosshairCanvas;
    private CanvasScaler canvasScaler;
    [SerializeField] GameObject crosshair;
   [SerializeField] Camera cam;
    [SerializeField] Suppression suppressionScript;


    [SerializeField] Transform raycastObject;
    RaycastHit hit;

    [SerializeField] Animator animator;

    [SerializeField] LayerMask layerMask;

    [SerializeField] Collider[] characterColliders = new Collider[50];
    private Dictionary<Collider, Suppression> suppressionScripts = new Dictionary<Collider, Suppression>(); // Array to find character colliders when shooting



    





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()

    {


        //  animator = GetComponentInParent<Animator>();

        // cam = GetComponentInParent<Camera>();
        //initialgunAimWeight = gunAimConstraint.weight;
        //initialRightShoulderWeight = RightShoulderAimConstraint.weight;

        if (transform.root.CompareTag("Player"))
        {
            canvasScaler = CrosshairCanvas.GetComponent<CanvasScaler>();
            crosshairWidth = crosshairTransform.sizeDelta.x;
            crosshairHeight = crosshairTransform.sizeDelta.y;
        }

        //layerMask = LayerMask.GetMask("Hitbox");
      //  Debug.Log("Layer mask " + layerMask.value);



        // Debug.Log("The current crosshair width is: " + crosshairWidth);
        // Debug.Log("The current crosshair height is: " + crosshairHeight);



    }

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Hitbox");
        initialgunAimWeight = gunAimConstraint.weight;
        initialRightShoulderWeight = RightShoulderAimConstraint.weight;
    }


    
    private void suppressionCandidates(Vector3 Origin, float radius) {

        Debug.Log("I am running this function currently!");
        int numColliders = Physics.OverlapSphereNonAlloc(Origin, radius, characterColliders);

        for (int i = 0; i < numColliders; i++) {

            Collider collider = characterColliders[i];
            if ((collider.CompareTag("Enemy") || collider.CompareTag("Player")) && (!collider.CompareTag(transform.root.tag))) { 
           
                //math to find the shortest distance from a collider to the ray of the bullet
               Vector3 lineToTarget = collider.transform.position - Origin; //find the distance between any point on the line and the collider
                Vector3 bulletDirection = (raycastObject.forward + Spread); //get the direction of the bullet
                 Vector3 closestDistanceToCollider = Vector3.Cross(lineToTarget, bulletDirection.normalized); //use cross product to find the perpendicular between the line and collider
                                                                                                  //the perpendicular is the shortest path between the two
                float distance = closestDistanceToCollider.magnitude; //get the magnitude of the perpendicular to find the shortest distance between the line and the collider

                float dotProduct = Vector3.Dot(lineToTarget, bulletDirection.normalized); //use dot product to find if the collider is in front of the bullet or behind it

                if (dotProduct < 0)
                {
                       targetInFront = false;

                }
                else
                {
                    targetInFront = true;
                }


                if (!suppressionScripts.TryGetValue(collider, out suppressionScript))
                {

                    suppressionScript = collider.GetComponent<Suppression>();


                    suppressionScripts.Add(collider, suppressionScript);


                }
                else if (suppressionScripts.TryGetValue(collider, out suppressionScript)) {

                    Debug.Log("The collider the script is attached to is:" + collider.name);
                    suppressionScript.ApplySuppression(targetInFront, distance);
                
                } 
                





            }


        }


    }


    //  IEnumerator RecoilReset()
    // {


    //   while (timeElapsed < timeToReset)
    //  {
    //    resetTracker = Mathf.Lerp(shotsForSpread, 0, timeElapsed / timeToReset);
    //   timeElapsed += Time.deltaTime;


    //  yield return null;

    // }
    // resetTracker = 0;


    // bulletSpreadFactor = 0;



    //   }

    public void Shoot()
    {
        bulletSpreadFactor = Mathf.Clamp(bulletSpreadFactor, minSpread, maxSpread);
        bulletSpreadFactor += bulletSpreadIncrement;
        


        Debug.Log("This function is being called");
      //  suppressionCandidates(raycastObject.position, 10f);

        if (!isReloading)
        {
            if (ammoInClip > 0)
            {

                shotsForSpread++;
                timeElapsed = 0;
                //  StartCoroutine(RecoilReset());
                //animator.SetBool("isFiring", true);
              //  Debug.Log(animator.GetBool("isFiring"));
              //  Debug.Log("I am shooting");

               

                bulletSpreadRadius = Random.insideUnitCircle * bulletSpreadFactor;
             //  Spread = (bulletSpreadRadius.x * cameraObject.right) + (bulletSpreadRadius.y * cameraObject.up);

               //bulletSpreadRadius = new Vector3(0.005f, 0, 0);
                
                Spread = (bulletSpreadRadius.x * raycastObject.right) + (bulletSpreadRadius.y * raycastObject.up);
               

                muzzleflash.Play();
                recoil.recoil();

               // Debug.Log("layer mask: " + layerMask.value);

                if (Physics.Raycast(raycastObject.position, raycastObject.forward + Spread, out hit, 40f))
                {

                    Debug.Log("The direction the bullet is traveling: " + (raycastObject.forward + Spread));
                    

                    bulletClone = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
                    bulletClone.transform.rotation *= Quaternion.Euler(0f, 0f, Random.Range(0, 360));

                    Debug.Log("You hit " + hit.collider.name);

                   
                   target = hit.transform.GetComponentInParent<ObjectHealth>();
                    
                    hitbox = hit.transform.GetComponent<HitboxDamage>();

                    if (hitbox != null)
                    {
                        hitboxMultiplier = hitbox.multiplier;
                    }
                    if (hitbox == null) {

                        Debug.Log("There is no collider");
                    
                    
                    }
                    if (target != null && hitbox != null && !hit.transform.CompareTag(transform.root.tag))
                    {

                       // Debug.Log("Found component");
                       // Debug.Log("The multiplier is: " + hitboxMultiplier);
                        target.takeDamage(bulletDamage * hitboxMultiplier, transform.root.gameObject);
                        

                    }


                    //    Debug.Log("Distance from target" + hit.distance);


                }

                // if (Mathf.Round(hit.distance % 10) == 0 && Mathf.Round(hit.distance) >= 10)
                // {
                //    bulletSpreadFactor += 0.005f;
                //  Debug.Log("This object is at a reasonable distance");


                // }



            }










            if (ammoInClip != 0 && ammoReserve != 0)
            {
                ammoInClip--;
                bulletsFired++;
            }
            else if (ammoReserve == 0 && ammoInClip > 0)
            {
                ammoInClip--;



            }

        }

        suppressionCandidates(raycastObject.position, 10f);





    }

    IEnumerator Reloading(int layerIndex)
    {

        var sources = spineAim.data.sourceObjects;
        isReloading = true;

        animator.SetBool("isReloading", isReloading);

        leftHandIK.weight = 0f;
        gunAimConstraint.weight = 0f;
        RightShoulderAimConstraint.weight = 0f;
        sources.SetWeight(0, 0.1f);
        spineAim.data.sourceObjects = sources;




        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Reload"));
        float animationLength = animator.GetCurrentAnimatorStateInfo(layerIndex).length;
        yield return new WaitForSeconds(animationLength - 0.25f);
       
        leftHandIK.weight = 1f;
        gunAimConstraint.weight = initialgunAimWeight;
        Debug.Log("Changing weights");

        RightShoulderAimConstraint.weight = initialRightShoulderWeight;
        var sourcesEnd = spineAim.data.sourceObjects;
        sourcesEnd.SetWeight(0, 1);
        spineAim.data.sourceObjects = sourcesEnd;




        if (ammoReserve / bulletsFired > 1)
        {
            ammoReserve -= bulletsFired;
            ammoInClip = ammoInClipMax;
            bulletsFired = 0;
        }
        else
        {
            ammoInClip = ammoReserve;
            ammoReserve = 0;
            bulletsFired = 0;




        }

       
        isReloading = false;
        animator.SetBool("isReloading", isReloading);


    }


    public void reload(int layerIndex)
    {

        if (ammoInClip < ammoInClipMax && ammoReserve != 0)
            StartCoroutine(Reloading(layerIndex));





    }


    private void updateCrosshairVisual(float bulletSpreadFactor)
    {
        resolutionFactor = CrosshairCanvas.scaleFactor;

        if (bulletSpreadFactor != maxSpread)
        {

            centerPosPixels = cam.WorldToScreenPoint(raycastObject.position + raycastObject.forward);
            Debug.Log("Center pos coordinates:" + centerPosPixels);
            radiusPosPixels = cam.WorldToScreenPoint(raycastObject.position + raycastObject.forward + (raycastObject.right * bulletSpreadFactor));
            // Debug.Log("Radius pos coordinates:" + radiusPosPixels);
            pixelDistance = Vector2.Distance(radiusPosPixels, centerPosPixels) / resolutionFactor;

            // Debug.Log("Distance between pixel points is: " +  pixelDistance);
            pixelDiameter = pixelDistance * 3.5f ;

            crosshairTransform.sizeDelta = new Vector2(pixelDiameter + crosshairWidth, pixelDiameter + crosshairHeight);
            maxCrosshairSpread = crosshairTransform.sizeDelta;



            //if (crosshairTransform.sizeDelta != maxCrosshairSpread)
            //{

            //    crosshairTransform.sizeDelta = maxCrosshairSpread;


            //}
        }
    }


    private void Update()
    {

        updateCrosshairVisual(bulletSpreadFactor);

        // Debug.DrawRay(raycastObject.position, (raycastObject.forward + Spread) * 200f, Color.yellow);
        //   Debug.Log("Root node: " + transform.root.gameObject.name);

        // bulletSpreadRadiusIncrement = Mathf.Clamp(bulletSpreadRadiusIncrement, minSpread, maxSpread);


        float currentCrosshairWidth = 0;
        float currentCrosshairHeight = 0;
        if (transform.root.CompareTag("Player"))
        {
            currentCrosshairWidth = crosshairTransform.sizeDelta.x;
            currentCrosshairHeight = crosshairTransform.sizeDelta.y;
        }

        Destroy(bulletClone, 5);

        // Debug.DrawRay(raycastObject.position, (raycastObject.forward + bulletSpreadRadius) * 200f, Color.red);


        //timeElapsed += Time.deltaTime;
        if (animator.GetBool("isFiring") == false)
        {
            Debug.Log("This should be happening");
            bulletSpreadFactor -= decayRate * Time.deltaTime;

            if (bulletSpreadFactor < 0)
            {


                bulletSpreadFactor = 0;

            }
            //if (transform.root.CompareTag("Player"))
            //{
            //    Debug.Log("Trying to do this");
            //    crosshairTransform.sizeDelta = Vector2.MoveTowards(crosshairTransform.sizeDelta, new Vector2(crosshairWidth, crosshairHeight), 3);

            //    //currentCrosshairHeight = Mathf.Lerp(currentCrosshairHeight, crosshairHeight, timeElapsed / timeToReset);
            //    //currentCrosshairWidth = Mathf.Lerp(currentCrosshairWidth, crosshairWidth, timeElapsed / timeToReset);

            //    crosshairTransform.sizeDelta = new Vector2(currentCrosshairWidth, currentCrosshairHeight);
            //}





        }



            //if (shotsForSpread <= 0.01f)
            //{

            //    bulletSpreadFactor = 0;
            //    addSpread = false;



            //}




            // Update is called once per frame
            //void Update()
            //{
            //    if (animator.GetBool("isFiring")) {

            //        if (Physics.Raycast(cameraObject.position, cameraObject.forward, out hit, 10.0f)) {

            //            Debug.Log("You hit " + hit.collider.name);



            //        }


            //    }



            //}
        }
    
}