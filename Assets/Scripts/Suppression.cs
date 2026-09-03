using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Suppression : MonoBehaviour
{
    [SerializeField] public float suppressionAmount = 0.01f;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private Gun currentGunScript; //the script of the current gun that is being used by the character,
                                                   //this will be used to access the bullet spread factor and max spread of the gun[
    [SerializeField] private float suppressionDecayRate = 0.02f; // The rate at which suppression decays over time
                                                                  // [SerializeField] private float maxSuppresion = 0.5f; // The maximum suppression amount
    public bool isSuppressed = false; // Flag to indicate if the character is currently suppressed
    public float maxSuppression = 0.15f; // The maximum suppression amount



    // Start is called once before the frst execution of Update after the MonoBehaviour is created
    void Start()
    {

        
            
          
        
    }

    // Update is called once per frame
    void Update()
    {
        
        currentGunScript = weaponInventory.currentGunScript;
        suppressionAmount -= suppressionDecayRate * Time.deltaTime;
        suppressionAmount = Mathf.Clamp(suppressionAmount, 0.0f, maxSuppression);



    }


    public void ApplySuppression(bool targetInFront, float bulletCloseness) {

        Debug.Log("The bullet spread factor is:" + weaponInventory.currentGunScript.bulletSpreadFactor + "and the character name is:" + transform.root.name );

        if (!targetInFront)
        {
            isSuppressed = false;
            return;

        }

        else {

            if (bulletCloseness <= 5.0f && bulletCloseness > 3.0f)
            {

                Debug.Log("we are applying suppression");
                suppressionAmount += 0.001f;

            }

            else if (bulletCloseness <= 3.0f && bulletCloseness > 1.5f) {

                suppressionAmount += 0.005f;



            }


            else if (bulletCloseness <= 1.5f && bulletCloseness > 0.5f){

                suppressionAmount += 0.01f;
            
            
            }


            if (weaponInventory != null && weaponInventory.currentGunScript != null)
            {
                Debug.Log("Trying to apply suppression");

                if(currentGunScript.bulletSpreadFactor + (suppressionAmount * 0.5f) < currentGunScript.maxSpread )  
                    currentGunScript.bulletSpreadFactor += (suppressionAmount * 0.5f);

                else
                    currentGunScript.bulletSpreadFactor = currentGunScript.maxSpread ;


               







            }







        }

       
        



    }




}
