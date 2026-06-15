using UnityEngine;

public class AimingHandler : MonoBehaviour
{

    public Transform aimTarget;
    Camera cam;
    Animator animator;
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform aimCam;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam  = GetComponent<Camera>();
        animator = playerController.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        //Debug.
        //(ray.origin, ray.direction, Color.green);

            if (Physics.Raycast(ray, out hit, 20f))
            {
                print("I'm looking at " + hit.transform.name);
                aimTarget.position = hit.point;


            }
            else
            {
                aimTarget.position = cam.transform.position + (cam.transform.forward * 20f);
                print("I'm looking at nothing!");
            }


        

    

            

        
        

    }
    
}
    



