using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{


    [SerializeField] Vector2 MouseRotation;
    public Vector3 LookRotation;
    public float LookSpeed;
    public Transform aimTarget;
    public Transform freeLookCam;
    public Vector3 freelookEuler;
    public Transform aimCam;
    public Vector3 targetPoint;
    public Quaternion aimCamRotation;
    public Vector3 initialPosition;
    [SerializeField] Transform shoulderFollow;
    RaycastHit hit; 
   


    private bool isSnapped = false; 


    private PlayerController playerController;
    [SerializeField] WeaponRecoil recoil;


    [SerializeField] Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       GameObject player = GameObject.FindWithTag("Player");
       playerController = player.GetComponentInParent<PlayerController>();
       // animator = playerController.GetComponent<Animator>();

        initialPosition = transform.localPosition;

    }


    void cameraRotation(Vector2 MouseRotate)
    {

       

        if (Mouse.current.rightButton.wasPressedThisFrame) { 
           // Debug.Log("Button is pressed");

           // transform.parent.rotation = transform.parent.rotation * freeLookCam.rotation;

        }
        //transform.rotation = Quaternion.Slerp(transform.rotation, freeLookCam.rotation, 10 * Time.deltaTime);
        if (animator.GetBool("isAiming"))
        {
           // Quaternion freeLookRotationY = Quaternion.Euler(0f, freeLookCam.eulerAngles.y, 0f);
          //  Quaternion freeLookRotationX = Quaternion.Euler(freeLookCam.eulerAngles.x, 0f, 0f);

            if (!isSnapped)
            {
                if (Physics.Raycast(freeLookCam.position, freeLookCam.forward, out hit, 200f))
                {


                   // targetPoint = hit.point;



                }

                else {
                   // targetPoint = freeLookCam.position + (freeLookCam.forward * 200f);

                
                
                
                }



                   LookRotation.y = freeLookCam.eulerAngles.y;
              LookRotation.x = freeLookCam.eulerAngles.x;
               //LookRotation.z = freeLookCam.eulerAngles.z;
  

                if (LookRotation.x > 300) {
                    LookRotation.x -= 360;
                
                
                }


           //   transform.localRotation = Quaternion.Euler(LookRotation.x, 0f, 0f);
             // transform.parent.rotation = Quaternion.Euler(0f, LookRotation.y, 0f);


               // Debug.Log("Value of LookRotationX before snapping: " + LookRotation.x);
                



                isSnapped = true;

            }


               






            // transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, freeLookCam.rotation, 10 * Time.deltaTime);
            //transform.parent.LookAt(aimTarget);
            // if (MouseRotate.sqrMagnitude < 0.01f)
            // {

            //    return;
            //}

            var scaledRotateSpeed = LookSpeed * Time.deltaTime;

            LookRotation.y += MouseRotate.x * scaledRotateSpeed;
            // Debug.Log("LookRotationX right before it changes: " + LookRotation.x);
            if (animator.GetBool("isFiring"))
            {
              //  Debug.Log(animator.GetBool("isFiring"));
               LookRotation.x = Mathf.Clamp(LookRotation.x - (MouseRotate.y + recoil.upRecoilWeaponOffset) * scaledRotateSpeed, -90, 90);
               // Debug.Log("IsFiring");


            }
            else
                LookRotation.x = Mathf.Clamp(LookRotation.x - MouseRotate.y * scaledRotateSpeed, -45, 45);
              //  Debug.Log(animator.GetBool("isFiring"));


            // Debug.Log("Value of LookRotationx after snapping: " + LookRotation.x);
           transform.localRotation = Quaternion.Euler(LookRotation.x, 0f, 0f);
            transform.parent.rotation = Quaternion.Euler(0f, LookRotation.y, 0f);

          
           




            // transform.parent.rotation = 
            // Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);


            // Vector3 parentEuler = transform.parent.localEulerAngles;
            // parentEuler.y = transform.localEulerAngles.y;  // copy only child's Y
            // transform.parent.localEulerAngles = parentEuler;


            //Vector3 parentEuler = transform.parent.eulerAngles;
            //parentEuler.y = transform.eulerAngles.y;   // match child's world Y rotation
            //transform.parent.eulerAngles = parentEuler;

        }

        else {

            isSnapped = false;
        
        
        }

        
    }
    // Update is called once per frame
    void LateUpdate()
    {
        MouseRotation = playerController.MouseRotation;
        cameraRotation(MouseRotation);


        //
        //(freeLookCam.position, freeLookCam.forward * 10f, Color.green);
        //Debug.DrawRay(transform.position, transform.forward * 10f, Color.yellow);

        aimCamRotation = aimCam.rotation;
        freelookEuler = freeLookCam.eulerAngles;

        if (animator.GetBool("isCrouched"))
        {
            transform.position = shoulderFollow.position;



        }
        else transform.localPosition = initialPosition;

        
    }



}


