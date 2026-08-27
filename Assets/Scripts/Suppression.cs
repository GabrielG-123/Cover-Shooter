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

            if (bulletCloseness <= 5.0f && bulletCloseness >= 0.05f) {

                Debug.Log("we are applying suppression");
                suppressionAmount += 0.0025f;
            
            }

            if (weaponInventory != null && weaponInventory.currentGunScript != null)
            {
                Debug.Log("Trying to apply suppression");
               // weaponInventory.currentGunScript.bulletSpreadRadiusIncrement += (suppressionAmount * 0.05f);
            }




        }

       
        



    }




}
