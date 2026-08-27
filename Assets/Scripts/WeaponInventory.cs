using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class WeaponInventory : MonoBehaviour
{

  //  public GameObject[] weapons;
    public List<GameObject> weapons = new List<GameObject>();

    public int currentweapon = 0;
    public Gun currentGunScript;


    int weaponIndex;

    [SerializeField] Transform weaponGrip;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    public RigBuilder rigBuilder;
    [SerializeField] Transform leftHandPosition;
    [SerializeField] Transform leftHandTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject weapon in weapons) {

            if (weapon != weapons[currentweapon]) { 
                weapon.SetActive(false);
            
            }
        
        }
        weapons[currentweapon].SetActive(true);


        leftHandPosition = weapons[currentweapon].transform.Find("ref_left_hand_gri");
        Debug.Log("The name of the current weapon is: " + weapons[currentweapon].name);

        currentGunScript = weapons[currentweapon].GetComponentInChildren<Gun>();

        if(currentGunScript == null) {
            Debug.LogError("The current weapon does not have a Gun script attached.");
        }


    }

    // Update is called once per frame
    void LateUpdate()
    {

        leftHandTarget.position = leftHandPosition.position;
        leftHandTarget.rotation = leftHandPosition.rotation;
    }


    //private void Update()
    //{
    //    if (weapons[currentweapon] != null && )
    //    {
    //        currentGunScript = weapons[currentweapon].GetComponent<Gun>();
    //    }
    //}

    public void switchWeapons(float mouseinput)
    {

        
        if (mouseinput > 0) {
            weapons[currentweapon].SetActive(false);
            currentweapon++;
           // Debug.Log(weapons[currentweapon].name);
            if (currentweapon > weapons.Count - 1)
            {

                currentweapon = 0;

            }

            if (weapons[currentweapon].transform.parent == null)
            {
                weapons[currentweapon].transform.SetParent(weaponGrip, false);
                weapons[currentweapon].SetActive(true);


            }
            else
            {
                weapons[currentweapon].SetActive(true);
                
                //Debug.Log("This is the current weapon: " +  weapons[currentweapon].name);
            }

            
           

            


        }


        if (mouseinput < 0) {
            weapons[currentweapon].SetActive(false);
            currentweapon--;

          //  Debug.Log(weapons[currentweapon].name);

            if (currentweapon < 0)
            {

                currentweapon = weapons.Count - 1;

            }

            weapons[currentweapon].SetActive(true);



        }
        leftHandPosition = weapons[currentweapon].transform.Find("ref_left_hand_gri");
        
        currentGunScript = weapons[currentweapon].GetComponentInChildren<Gun>();






    }


}
