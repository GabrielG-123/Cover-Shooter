using Mono.Cecil.Cil;
using System.Collections;
using System.Runtime.CompilerServices;
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
    private Vector3 Spread;
    private Vector3 centerPosPixels;
    private Vector3 radiusPosPixels;
    private Vector2 maxCrosshairSpread;
    private float pixelDistance;
    private float pixelDiameter;
    private float crosshairWidth;
    private float crosshairHeight;
    public float bulletSpreadFactor;
    public float bulletSpreadRadiusIncrement;
    public float shotsForSpread;
    public float timeToReset;
    public float timeElapsed;
    public float resetTracker;
    public float minimumShotsBeforeSpread;
   public bool addSpread = false;
    private float resolutionFactor;
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


    [SerializeField] Transform raycastObject;
    RaycastHit hit;

    [SerializeField] Animator animator;

    [SerializeField] LayerMask layerMask;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()

    {

        
      //  animator = GetComponentInParent<Animator>();
        canvasScaler = CrosshairCanvas.GetComponent<CanvasScaler>();
        
       // cam = GetComponentInParent<Camera>();
        //initialgunAimWeight = gunAimConstraint.weight;
        //initialRightShoulderWeight = RightShoulderAimConstraint.weight;
       


        crosshairWidth = crosshairTransform.sizeDelta.x;
        crosshairHeight = crosshairTransform.sizeDelta.y;

        //layerMask = LayerMask.GetMask("Hitbox");
        Debug.Log("Layer mask " + layerMask.value);



        // Debug.Log("The current crosshair width is: " + crosshairWidth);
        // Debug.Log("The current crosshair height is: " + crosshairHeight);



    }

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Hitbox");
        initialgunAimWeight = gunAimConstraint.weight;
        initialRightShoulderWeight = RightShoulderAimConstraint.weight;
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

                if (Physics.Raycast(raycastObject.position, raycastObject.forward + Spread, out hit, 200f))
                {

                    

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
                    if (target != null && hitbox != null)
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
        Debug.Log("We are here");
        leftHandIK.weight = 1f;
        gunAimConstraint.weight = initialgunAimWeight;
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


    private void Update()
    {
       // Debug.DrawRay(raycastObject.position, (raycastObject.forward + Spread) * 200f, Color.yellow);
     //   Debug.Log("Root node: " + transform.root.gameObject.name);

        float currentCrosshairWidth = crosshairTransform.sizeDelta.x;
        float currentCrosshairHeight = crosshairTransform.sizeDelta.y;
        Destroy(bulletClone, 5);

       // Debug.DrawRay(raycastObject.position, (raycastObject.forward + bulletSpreadRadius) * 200f, Color.red);


        timeElapsed += Time.deltaTime;
        if (animator.GetBool("isFiring") == false)
        {
            shotsForSpread = Mathf.Lerp(shotsForSpread, 0, timeElapsed / timeToReset);
            if (shotsForSpread > 0)
            {
                currentCrosshairHeight = Mathf.Lerp(currentCrosshairHeight, crosshairHeight, timeElapsed / timeToReset);
                currentCrosshairWidth = Mathf.Lerp(currentCrosshairWidth, crosshairWidth, timeElapsed / timeToReset);

                crosshairTransform.sizeDelta = new Vector2(currentCrosshairWidth, currentCrosshairHeight);
            }





        }



        if (shotsForSpread <= 0.01f)
        {

            bulletSpreadFactor = 0;
            addSpread = false;



        }

        else if (shotsForSpread > minimumShotsBeforeSpread)
        {

            if (!addSpread)
            {
                resolutionFactor = CrosshairCanvas.scaleFactor;
                addSpread = true;
                bulletSpreadFactor += bulletSpreadRadiusIncrement;
                centerPosPixels = cam.WorldToScreenPoint(raycastObject.position + raycastObject.forward);
               // Debug.Log("Center pos coordinates:" + centerPosPixels);
                radiusPosPixels = cam.WorldToScreenPoint(raycastObject.position + raycastObject.forward + (raycastObject.right * bulletSpreadFactor) );
               // Debug.Log("Radius pos coordinates:" + radiusPosPixels);
                pixelDistance = Vector2.Distance(radiusPosPixels, centerPosPixels) / resolutionFactor;

               // Debug.Log("Distance between pixel points is: " +  pixelDistance);
                pixelDiameter = pixelDistance * 3.5f;

                crosshairTransform.sizeDelta = new Vector2(pixelDiameter + crosshairTransform.sizeDelta.x, pixelDiameter + crosshairTransform.sizeDelta.y);
                maxCrosshairSpread = crosshairTransform.sizeDelta;




                




            }


            if (addSpread && animator.GetBool("isFiring") && crosshairTransform.sizeDelta != maxCrosshairSpread) { 
                    
                crosshairTransform.sizeDelta = maxCrosshairSpread;
            
            
            }






        }




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