using UnityEngine;
using UnityEngine.Rendering;

public class WeaponRecoil : MonoBehaviour
{

   //public float kickbackspeed = ;
   public float returnspeed = -0.5f;
    public float xValue;
    public float recoilBackPosition;
    public float recoilForwardPosition;
    public float t;
    public float u;
    public float deltaTime;
    public float frameRate;
    public float verticalRecoilWeaponOffset;
    

    public Animator animator;
    public float upRecoilWeaponOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    public void recoil()
    {

        verticalRecoilWeaponOffset += -0.05f;
        





    }

    // Update is called once per frame
    void Update()
    {

        t = 1f - (1f / (20f * Time.deltaTime + 1f));
        u = 1f - (1f / (20f * Time.deltaTime + 1f));

        transform.localPosition = new Vector3(verticalRecoilWeaponOffset, transform.localPosition.y, transform.localPosition.z);


        verticalRecoilWeaponOffset = Mathf.Lerp(verticalRecoilWeaponOffset, 0, t);

        transform.localPosition = new Vector3(verticalRecoilWeaponOffset, transform.localPosition.y, transform.localPosition.z);

        // if (animator.GetBool("isFiring") == false) upRecoilWeaponOffset = Mathf.Lerp(upRecoilWeaponOffset, 0, u);


        if (animator.GetBool("isFiring") == false && transform.localPosition.x <= 0.0001f) {
         // Debug.Log("This x value is 0");
           // transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
        
        
       }


           


        // recoilForwardPosition = Mathf.Lerp(recoilBackPosition, transform.lo, 10f * Time.deltaTime);
        // transform.localPosition = new Vector3(recoilForwardPosition, transform.localPosition.y, transform.localPosition.z);
      
       // t = Mathf.Clamp(t, 0.3f, 0.9f);
        frameRate = 1 / Time.deltaTime;
        deltaTime = Time.deltaTime;


        //Debug.Log(Mathf.Epsilon);



        // xValue = Mathf.Lerp(0, 1, 0.5f * Time.deltaTime);

        //transform.localPosition = new Vector3(Mathf.Lerp(0, recoilPosition , t), transform.localPosition.y, transform.localPosition.z);
        // transform.localPosition += new Vector3(returnvalue * Time.deltaTime, 0f, 0f);


       // Debug.Log("Value of offset: " + verticalRecoilWeaponOffset);
    }

    
}
