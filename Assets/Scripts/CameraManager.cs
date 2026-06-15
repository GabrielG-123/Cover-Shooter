using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineCamera[] cameras; 

    public CinemachineCamera freelookCam;
    public CinemachineCamera aimingCam;

    public Quaternion freeLookCamRotation;
    public Quaternion aimCamRotation;

    public CinemachineCamera startingCam;
    private CinemachineCamera currentCam;

    [SerializeField] PlayerController player;


    float currValue = 1f;
    int currValueInt;
    float transitionRate = 0.95f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    private void Awake()
    {
        currentCam = startingCam;

        currentCam.Priority.Value = 10;

        player.cameraObject = currentCam.transform;
    }

    // Update is called once per 
    void Update()
    {
        freeLookCamRotation = freelookCam.transform.rotation;
        aimCamRotation = aimingCam.transform.rotation;

    }


    public void BlendToCam(CinemachineCamera newCam) {




       


        currentCam = newCam;
        currentCam.Priority.Value = 10;

        player.cameraObject = currentCam.transform;

        
       

        

        

        

        for (int i = 0; i < cameras.Length; i++) {


            if (cameras[i] != currentCam)
            {
                cameras[i].Priority = 1;
            
            
            }

        
        }
        
    
    
    }
}
