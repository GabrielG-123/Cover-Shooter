using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;



public class PlayerController : MonoBehaviour

{

    public Vector2 MoveDirection;
    public Vector3 ThreeDMoveDirection;
    public Vector3 ThreeDCoverMoveDirection;
    public Vector3 gravityMove;
    public Vector3 SprintDirection;
    public Vector3 LookDirection;
    public Vector2 LookRotation;
    public Vector2 MouseRotation;
    float mouseScroll;


    [SerializeField] Vector3 oppositeNormal;
    public Vector2 CoverMoveDirection;
    private PlayerInput playerInput;
    [SerializeField] private bool isMoving;
    [SerializeField] Vector3 rightTransform;

    private bool isCrouched;
    private bool isWalking;
    private bool isSprinting;
    private bool isStanding;
    private bool canTakeCover;
    private bool isMovingCover;
    private bool inCover;
    private bool sneakingLeft;
    private bool sneakingRight;
    private bool ctrlPressed;
    private bool shiftPressed;
    private bool isMovingCrouched;
    private bool exitCover;
    private bool isPeeking;
    private bool vault;
    private bool canVault;
    private bool isGrounded;
    private bool isAiming;
    private bool isFiring;
    private bool isShooting;
    private bool isReloading;
    

    private string currentMap;



    CharacterController characterController;
    public float moveSpeed = 10.0f;
    Animator animator;
    [SerializeField] private float rotationFactor = 9.0f;
    public float checkValue = 0.2f;
    private float coverAngle;
    private float ySpeed;
    [SerializeField] float LookSpeed;
    int currentWeapon;



    private int MoveXHash;
    private int MoveZHash;

    int groundLayer;



    //ICinemachineCamera currentCam;
   // [SerializeField] CinemachineBrain cameraBrain;
    public Transform cameraObject;
    [SerializeField] Transform thirdPersonCamera;
    [SerializeField] private Vector3 forwardTransform;
    [SerializeField] private Vector3 horizontalTransform;
    [SerializeField] private Vector3 playerRight;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Transform coverRaycastLeft;
    [SerializeField] Transform coverRaycastRight;
    [SerializeField] Transform gunRaycast;
    private Vector3 moveVectorHorizontal;
    private Vector3 moveVectorVertical;
    [SerializeField] private Vector3 moveVector;
    [SerializeField] private Vector3 gravityMovement;
    [SerializeField] private Transform aimTarget;
    [SerializeField] Vector3 aimTargetPosition;
    [SerializeField] Gun myGun;
    public GameObject crosshair;
    WeaponInventory inventory;
    [SerializeField] WeaponRecoil recoil;
    [SerializeField] Camera weaponCam;
    [SerializeField] Camera mainCam;
    [SerializeField] Transform playerModel;
    


    [SerializeField] CameraManager camManager;


    [SerializeField] Quaternion currentRotation;
    [SerializeField] Quaternion coverRotation;



    RaycastHit hit;
    RaycastHit coverHit;
    [SerializeField] Vector3 coverHitPoint;
    [SerializeField] float percentComplete;


    void Start()
    {
        forwardTransform.y = 0f;
        horizontalTransform.y = 0f;

        crosshair.SetActive(false);
        weaponCam.enabled = false;

    }

    private void Awake()
    {

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        inventory = GetComponent<WeaponInventory>();
        groundLayer = LayerMask.GetMask("Ground");

        MoveXHash = Animator.StringToHash("MoveX");
        MoveZHash = Animator.StringToHash("MoveZ");


        // animator.SetFloat("MoveX", 1.0f);
        //  animator.SetFloat("MoveZ", 0.5f);


    }


    private void OnWalk(InputValue value) {


        if (!isSprinting)
        {
            MoveDirection = value.Get<Vector2>();
            ThreeDMoveDirection.x = MoveDirection.x;
            ThreeDMoveDirection.z = MoveDirection.y;
            isMoving = MoveDirection.x != 0 || MoveDirection.y != 0;
            
        

    } else {
        // When sprinting, no directional input from WASD allowed
        MoveDirection = Vector2.zero;
        isMoving = false;
       // animator.SetBool("isWalking", false);
    }








    }

    private void OnSprint(InputValue value)
    {

        shiftPressed = value.isPressed;
        // animator.SetBool("isSprinting", true);





    }

    private void OnCover() {
        if (CheckForCover())
        {
            animator.SetBool("ExitCover", false);
            oppositeNormal = -coverHit.normal;
            coverHitPoint = coverHit.point;
            




            animator.SetBool("canTakeCover", true);
            StartCoroutine(WaitForCoverAnimation());
            Debug.Log("This function is running");

            animator.SetBool("inCover", true);









        }




    }

    private void OnCoverMovement(InputValue value)
    {
        if (!isAiming)
        {
            CoverMoveDirection = value.Get<Vector2>();
            isMovingCover = CoverMoveDirection.x > 0 || CoverMoveDirection.x < 0;
            ThreeDCoverMoveDirection = CoverMoveDirection.x * playerModel.right;

            if (CoverMoveDirection.x < 0 && ThreeDCoverMoveDirection.x != 0)
            {

                animator.SetBool("sneakingLeft", true);

                animator.SetBool("isMovingCrouched", true);

            }
            else if (CoverMoveDirection.x > 0)
            {

                animator.SetBool("sneakingRight", true);

                animator.SetBool("isMovingCrouched", true);
            }

            else
            {

                animator.SetBool("sneakingRight", false);
                animator.SetBool("sneakingLeft", false);
                animator.SetBool("isMovingCrouched", false);
            }



        }

       













    }


    private void OnExitCover() {
        animator.SetBool("ExitCover", true);
        playerInput.SwitchCurrentActionMap("BaseMovement");

        animator.SetBool("inCover", false);
        animator.SetBool("canTakeCover", false);




    }




    //private void isAimingDown() {





    //    if (!inCover)
    //    {

    //        if (Mouse.current.rightButton.wasReleasedThisFrame)
    //        {


    //            // crosshair.SetActive(false);
    //            animator.SetBool("isAiming", false);
    //            camManager.BlendToCam(camManager.freelookCam);
    //        }

    //        else if (Mouse.current.rightButton.wasPressedThisFrame)
    //        {
    //            //  crosshair.SetActive(true);
    //            animator.SetBool("isAiming", true);
    //            camManager.BlendToCam(camManager.aimingCam);


    //            // Debug.Log("Testing TESTING testing");


    //        }
    //    }
            
    
    
    //}

    private void OnAimDown(InputValue value) {

        bool isPressed = value.isPressed;
      //  Debug.Log("is pressed bool:" +  isPressed);
        if (isPressed)
        {

            // crosshair.SetActive(false);
            animator.SetBool("isAiming", true);

            camManager.BlendToCam(camManager.aimingCam);
            mainCam.cullingMask &= ~LayerMask.GetMask("Weapon");
            weaponCam.enabled = true;

            
        }

        else
        {
            //  crosshair.SetActive(true);
            animator.SetBool("isAiming", false);
            camManager.BlendToCam(camManager.freelookCam);
            mainCam.cullingMask = -1;
            weaponCam.enabled = false;
            


            // Debug.Log("Testing TESTING testing");


        }

        

    }


    private void OnCoverAim(InputValue value)
    {

        bool isPressed = value.isPressed;
       // Debug.Log("is pressed bool:" + isPressed);
        if (isPressed)
        {

            // crosshair.SetActive(false);
            animator.SetBool("isAiming", true);
           camManager.BlendToCam(camManager.aimingCam);

            mainCam.cullingMask &= ~LayerMask.GetMask("Weapon");
            weaponCam.enabled = true;
        }


        else
        {
            //  crosshair.SetActive(true);
            animator.SetBool("isAiming", false);
            camManager.BlendToCam(camManager.freelookCam);

            mainCam.cullingMask = -1;
            weaponCam.enabled = false;


            // Debug.Log("Testing TESTING testing");


        }



    }




    private void OnFire(InputValue value)
    {



        isShooting = value.isPressed;
       // animator.SetBool("isFiring", true);
        //Debug.Log(animator.GetBool("isFiring"));   //  recoil.recoil()

        

    }


    private void OnCoverShoot(InputValue value)
    {

        isShooting = value.isPressed;



    }

    public void OnLook(InputValue value) {

        MouseRotation = value.Get<Vector2>();
    
    
    }

    public void OnCoverLook(InputValue value)
    {

        MouseRotation = value.Get<Vector2>();


    }


    private void StopShooting() {

        if (!isShooting) {

            animator.SetBool("isFiring", false);
        
        
        }
    
    
    }

    private void OnReload()
    {

        myGun.reload(1);



    }

    void OnWeaponScroll(InputValue value) { 
        mouseScroll = value.Get<float>();
      //  Debug.Log("Scroll value "+ mouseScroll);

        inventory.switchWeapons(mouseScroll);

     
    
    
    }

    private bool CheckForCover()
    {
        //Debug.Log("This function is running");

        if (Physics.Raycast(raycastOrigin.position, playerModel.forward, out coverHit, 0.75f))
        {
            coverHitPoint = coverHit.point;
            if (coverHit.collider.CompareTag("Cover"))
            {

                Collider hitCollider = coverHit.collider;
               // Debug.Log("Edge:" + hit.collider.bounds.max);
                Debug.DrawRay(coverHit.collider.bounds.max, coverHit.normal * 2.0f, Color.yellow);
                Debug.DrawRay(coverHit.collider.bounds.min, coverHit.normal * 2.0f, Color.blue);
                Debug.DrawRay(coverHitPoint, -coverHit.normal * 10f, Color.green);
                Debug.Log("The direction of the normal vector is: " + coverHit.normal);



               // Debug.Log("You can take cover!");
                //Debug.Log("Normal direction: " + hit.normal);

                //coverAngle = Vector3.Angle(hit.normal, playerModel.forward);
                // Debug.Log("Angle " + coverAngle);


                return true;





            }





        }

        return false;
    }


    private void checkForCoverEdge()
    {

        if (inCover && !isAiming)
        {

            RaycastHit hit2;
            Debug.DrawRay(coverRaycastLeft.position, playerModel.forward * 1.0f, Color.black);
            Debug.DrawRay(coverRaycastRight.position, playerModel.forward * 1.0f, Color.cyan);
            if (Physics.Raycast(coverRaycastLeft.position, playerModel.forward, out hit2, 1.0f) == false)
            {

                animator.SetBool("sneakingLeft", false);
                //animator.CrossFade("Stand To Cover 0", 0.5f);
                Debug.Log("You cant move left!");
                if (CoverMoveDirection.x < 0)
                {
                    //Debug.Log("Left was pressed");
                    ThreeDCoverMoveDirection = Vector3.zero;
                    animator.SetBool("isPeeking", true);


                }
                else
                {

                    animator.SetBool("isPeeking", false);
                }



            }

            if (Physics.Raycast(coverRaycastRight.position, playerModel.forward, out hit2, 1.0f) == false)
            {

                Debug.Log("You cant move right! ");
                if (CoverMoveDirection.x > 0)
                {
                    ThreeDCoverMoveDirection = Vector3.zero;


                }

                animator.SetBool("sneakingRight", false);
             // animator.CrossFade("Stand To Cover 1", 0.5f);


            }
        }


    }

    private void coverAlignment()
    {
        if (inCover && !isAiming)
        {
            if (playerModel.forward != -coverHit.normal) { 
                playerModel.rotation = Quaternion.LookRotation(oppositeNormal, Vector3.up);



            }




        }




    }


    public void EnterCover() { 
        
    
    }

    private void checkForVault() {

        if (Physics.Raycast(raycastOrigin.position - new Vector3(0, 1, 0), playerModel.forward, out hit, 0.5f))
        {


            float height = hit.collider.bounds.size.y;

            if (height >= 1.1f && height <= 1.5f && (hit.normal == hit.collider.transform.forward || hit.normal == -hit.collider.transform.forward))
            {

                animator.SetBool("canVault", true);

            }
            else
            {
                animator.SetBool("canVault", false);


            }



        }
        else {

            animator.SetBool("canVault", false);


        }








    }


    private void OnVaultStanding() {
        
        if (animator.GetBool("canVault") == true)
        {
            animator.SetBool("vault", true);


        }


    }

    private void OnVaultInCover() {

        if (animator.GetBool("canVault")) {



            //  animator.applyRootMotion = true;
            animator.SetBool("vault", true);

            StartCoroutine(FinishVaulting());






        }


    }

    public void vaultingEvent()
    {

        animator.applyRootMotion = false;
        animator.SetBool("vault", false);


    }

    public void vaultingStartEvent() {

        animator.applyRootMotion = true;


    }

    bool groundedCheck()
    {

        if (Physics.CheckSphere(raycastOrigin.position - new Vector3(-0.001f, 0.87f, 0.05f), checkValue, groundLayer)) {


           Debug.Log("Is grounded");
            return true;

        }

        Debug.Log("Is Not Grounded");
        return false;

    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(raycastOrigin.position - new Vector3(-0.001f, 0.87f, 0.05f), checkValue);



    }

    IEnumerator WaitForCoverAnimation() {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Stand To Cover"));

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        float lerpDuration = 0.5f;
        float timeElapsed = 0;

       
        
        while (timeElapsed < lerpDuration) {

            playerModel.position = Vector3.Lerp(playerModel.position, new Vector3(coverHitPoint.x, playerModel.position.y, coverHitPoint.z), percentComplete);
            timeElapsed += Time.deltaTime;
            percentComplete = timeElapsed / lerpDuration;
         //   Debug.Log("transform position" + playerModel.position);
           // Debug.Log("coverhit position" + coverHit.point);

            yield return null;


        }

      //  playerModel.position = coverHit.point;
        yield return new WaitForSeconds(animationLength - 1.0f);

       // playerModel.position = coverHit.point;
       coverRotation = Quaternion.LookRotation(oppositeNormal, Vector3.up);
       playerModel.rotation = Quaternion.Slerp(currentRotation, coverRotation, 1);
       



        playerInput.SwitchCurrentActionMap("Cover");



    }

    IEnumerator FinishVaulting() {

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Vaulting"));

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationLength - 1.0f);

        animator.SetBool("inCover", false);
        animator.SetBool("canTakeCover", false);

        playerInput.SwitchCurrentActionMap("BaseMovement");


    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    private void AnimationManager() {
        isWalking = animator.GetBool("isWalking");
        isCrouched = animator.GetBool("isCrouched");
        isStanding = animator.GetBool("isStanding");
        isSprinting = animator.GetBool("isSprinting");
        canTakeCover = animator.GetBool("canTakeCover");
        inCover = animator.GetBool("inCover");
        sneakingLeft = animator.GetBool("sneakingLeft");
        sneakingRight = animator.GetBool("sneakingRight");
        isMovingCrouched = animator.GetBool("isMovingCrouched");
        exitCover = animator.GetBool("ExitCover");
        isPeeking = animator.GetBool("isPeeking");
        vault = animator.GetBool("vault");
        canVault = animator.GetBool("canVault");
        isAiming = animator.GetBool("isAiming");
        isFiring = animator.GetBool("isFiring");


        //  float animationLayerWeight = animator.GetLayerWeight(1);




        if (isMoving && !isSprinting) {
            animator.SetBool("isWalking", true);



        }





        if (!isMoving) {
            animator.SetBool("isMoving", false);
            animator.SetBool("isWalking", false);
        }

        if (isMoving) {
            animator.SetBool("isMoving", true);

        }

        if (!isCrouched && ctrlPressed) {

            animator.SetBool("isCrouched", true);
            animator.SetBool("isStanding", false);


        }

        if (isCrouched && ctrlPressed)
        {


            animator.SetBool("isStanding", true);
            animator.SetBool("isCrouched", false);


        }


        if (isMoving && isAiming)
        {

            animator.SetBool("isWalking", true);
            animator.SetBool("isSprinting", false);


        }

        else if (isMoving && shiftPressed)
        {

            animator.SetBool("isWalking", false);
            animator.SetBool("isSprinting", true);

            // MoveDirection = Vector2.Zero


        }



        //if(isMoving && shiftPressed) {
        //  animator.SetBool("isSprinting", true);

        // 
        // }

        if (!isMoving && shiftPressed)
        {
            animator.SetBool("isSprinting", false);



        }

        if (isMoving && isCrouched && shiftPressed)
        {

            animator.SetBool("isCrouched", false);

        }

        if (!shiftPressed)
        {

            animator.SetBool("isSprinting", false);


        }


        if (isAiming)
        {

            animator.SetLayerWeight(1, 1f);

           // if (isMoving)
           // {


           //     animator.SetLayerWeight(2, 1f);
           // 
           // else
            //    animator.SetLayerWeight(2, 0);


                animator.SetFloat(MoveXHash, MoveDirection.x);
            animator.SetFloat(MoveZHash, MoveDirection.y);


            //if (MoveDirection.x > 0 && MoveDirection.x < 1)
            //  {

            //    MoveDirection.x = 1;


            // }
            // else if (MoveDirection.x < 0 && MoveDirection.x > -1) {

            //    MoveDirection.x = -1;


            //}

            //if (MoveDirection.y > 0 && MoveDirection.y < 1) { MoveDirection.y = 1; }

            //else if(MoveDirection.y < 0 && MoveDirection.y > -1) {  MoveDirection.y = -1; }





        }
        else {


            animator.SetLayerWeight(1, 0f);


        }

        if (animator.GetBool("isReloading") == true)
        {


            animator.SetLayerWeight(1, 1f);


        }
       // else
           // animator.SetLayerWeight(1, 0);

       // Debug.Log("Weight of Weapon layer: " + animator.GetLayerWeight(1));
       // Debug.Log("Value of X: " + MoveDirection.x);
       // Debug.Log("Value of Y:" + MoveDirection.y);


       




    }

    private void CharacterRotation() {

        Vector3 lookAtPosition;
        lookAtPosition.x = ThreeDMoveDirection.x;
        lookAtPosition.y = 0.0f;
        lookAtPosition.z = ThreeDMoveDirection.z;


        currentRotation = playerModel.rotation;

        if (isMoving && !animator.GetBool("isAiming"))
        {

            Quaternion nextRotation = Quaternion.LookRotation(moveVector, Vector3.up);
            playerModel.rotation = Quaternion.Slerp(currentRotation, nextRotation, 10 * Time.deltaTime);

        }
        //





    }


    

    private void AimingRotation() {
        if (animator.GetBool("isAiming"))
        {
           

            Vector3 forward = cameraObject.transform.forward;
                forward.y = 0f;
               forward.Normalize();

            // Smoothly rotate player toward that direction
           // if (MouseRotate.sqrMagnitude > 0)
           // {
                //Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
               // playerModel.rotation = targetRotation;
          // playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, targetRotation, rotationFactor * Time.deltaTime);On

            //Quaternion targetRotation = Quaternion.Euler(0f, cameraObject.eulerAngles.y, 0f);

            // Smoothly rotate towards target yaw
            // Option A: Slerp with a speed factor
            //playerModel.rotation = Quaternion.Rotate(playerModel.rotation, targetRotation, Time.deltaTime * 10f);
            playerModel.LookAt(aimTarget, Vector3.up);

            //  }

        }

          //   Get camera's forward direction, but ignore vertical tilt
           
        }






    private void LateUpdate()
    {

        //AimingRotation();

    }




    // Update is called once per frame
    void Update()
    {

        
        playerRight = playerModel.right;
       // Debug.DrawRay(coverRaycastLeft.position, playerModel.forward * 1.0f, Color.black);
       // Debug.DrawRay(coverRaycastRight.position, playerModel.forward * 1.0f, Color.cyan);
        //   currentCam = cameraBrain.ActiveVirtualCamera;
        //if (groundedCheck())
        //{

        //    ySpeed = -0.5f;
        //    // Debug.Log("Is grounded!");



        //}
        //else
        //{

        //    ySpeed += Physics.gravity.y * Time.deltaTime;
        //    Debug.Log("Is in the air!");
        //    characterController.Move(gravityMovement * Time.deltaTime);



        //}


        isMoving = MoveDirection.x != 0 || MoveDirection.y != 0;

        SprintDirection = new Vector3(playerModel.forward.x, 0, playerModel.forward.z);

        if (animator.GetBool("vault")){

            return;
        }


        ctrlPressed = Keyboard.current.leftCtrlKey.wasPressedThisFrame;

       
        gravityMovement = new Vector3(0, ySpeed, 0);



        forwardTransform = cameraObject.transform.forward;
        horizontalTransform = cameraObject.transform.right;

        forwardTransform.y = 0;
        horizontalTransform.y = 0;

        forwardTransform = forwardTransform.normalized;
        horizontalTransform = horizontalTransform.normalized;

        moveVectorHorizontal = MoveDirection.x * horizontalTransform;
        moveVectorVertical = MoveDirection.y *  forwardTransform;
        moveVector = moveVectorHorizontal + moveVectorVertical;


       // Debug.Log("The move vector is: " + moveVector);


        


        rightTransform = playerModel.right;


        currentMap = playerInput.currentActionMap.name;


        Vector3 velocity = moveVector.normalized * moveSpeed;

       // Debug.Log("The player's velocity is: " + velocity);

        if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
        {
            Debug.Log("CRITICAL: Velocity became NaN! Resetting to zero.");
            velocity = Vector3.zero;
        }



        AnimationManager();
        CharacterRotation();
        coverAlignment();
        checkForCoverEdge();
        checkForVault();
        //isAimingDown();
        //AimingRotation();
        StopShooting();


       // aimTargetPosition = aimTarget.position;

       
        //aimTarget.position = Camera.main.playerModel.position + Camera.main.playerModel.forward * 10.0f;

        if (isWalking && (!shiftPressed || isAiming) )
        {
            characterController.Move(Time.deltaTime * velocity);

        }



        else if (isSprinting)
        {

            characterController.Move(SprintDirection * Time.deltaTime * moveSpeed * 3.0f);
           // Debug.Log("Shift is pressed!");

        }

        else if (isMovingCover)
        {
            
            characterController.Move(ThreeDCoverMoveDirection * Time.deltaTime * moveSpeed);
        }

        myGun = inventory.weapons[currentWeapon].GetComponentInChildren<Gun>();
        currentWeapon = inventory.currentweapon;
        //Calculate how many bullets we can shoot within a 1 second interval
        if (isShooting && Time.time > myGun.nextFire && isAiming)
        {

            if (myGun.ammoInClip > 0)
            {
                animator.SetBool("isFiring", true);
                myGun.nextFire = Time.time + myGun.fireRate;
                myGun.Shoot();
            }

            else animator.SetBool("isFiring", false);

          //  Debug.Log("Is shooting");




        }







        if (animator.GetBool("vault") == true) {

           CoverMoveDirection = Vector2.zero;

        
        
        }

        gravityMovement = new Vector3(0, ySpeed, 0);

        gravityMove = velocity + gravityMovement;


      //  Debug.Log(ctrlPressed);

        CheckForCover();
      //  Debug.DrawRay(raycastOrigin.position, playerModel.forward * 0.3f,Color.green);
       // Debug.DrawRay(hit.point, hit.normal * 2.0f, Color.red);
      // Debug.DrawRay(cameraObject.position, cameraObject.forward * 10.5f, Color.blue);
       // Debug.DrawRay(thirdPersonCamera.position, thirdPersonCamera.forward * 8.0f, Color.red);
      // Debug.DrawRay(gunRaycast.position, gunRaycast.forward * 10f, Color.blue);
     //   Debug.Log("The direction of the Z vector" + playerModel.forward.normalized);

       //  Debug.Log("Value of MoveX: " + animator.GetFloat("MoveX"));

       // Debug.Log("Current action map: " + playerInput.currentActionMap.name);

       //  Debug.Log("Current character velocity: " + characterController.velocity.magnitude);

      //  Debug.Log(shiftPressed);
        //  Debug.Log("Time since start of game: " + Time.time);

      //  Debug.DrawRay(playerModel.position, playerModel.forward * 20f, Color.blue);




        
        
        if (Physics.Raycast(raycastOrigin.position, playerModel.forward, out hit, 0.5f))
        {


            float height = hit.collider.bounds.size.y;
           // Debug.Log("This objects height is: " + height);


        }
















    }

  
}
