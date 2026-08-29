using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Suppression : MonoBehaviour
{
    [SerializeField] private float suppressionAmount = 0.01f;
    [SerializeField] private WeaponInventory weaponInventory;
    //access the inventory of weapons to get the current gun and its suppression value
   // private float bulletClosenessThreshold = 5.0f; // Distance threshold for bullet proximity
   // Collider[] characterColliders = new Collider[50]; // Array to hold character colliders

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
            
          
        
    }

    // Update is called once per frame
    void Update()
    {
        Mathf.Clamp(suppressionAmount, 0.0f, 0.5f);
        
    }


    public void ApplySuppression(bool targetInFront, float bulletCloseness) {

        Debug.Log("The bullet spread factor is:" + weaponInventory.currentGunScript.bulletSpreadFactor + "and the character name is:" + transform.root.name );

        if (!targetInFront)
        {

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

                if(weaponInventory.currentGunScript.bulletSpreadFactor < weaponInventory.currentGunScript.maxSpread)  
                    weaponInventory.currentGunScript.bulletSpreadFactor += (suppressionAmount * 0.5f);
            }




        }

       
        



    }




}
