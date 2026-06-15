using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{


    private TMP_Text ammoTracker;
    [SerializeField] Gun gun;
    [SerializeField] WeaponInventory inventory;
    int currentWeapon;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {

        ammoTracker = GetComponent<TMP_Text>();
        

    }

    // Update is called once per frame
    void Update()
    {
        currentWeapon = inventory.currentweapon;
        gun = inventory.weapons[currentWeapon].GetComponentInChildren<Gun>();
        ammoTracker.text = gun.ammoInClip + " / " + gun.ammoReserve;
    }
}
