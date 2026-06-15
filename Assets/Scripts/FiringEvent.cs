using UnityEngine;

public class FiringEvent : MonoBehaviour
{





    [SerializeField] StateManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
        
    }


    public void StartShooting() {

        
        
            Debug.Log("Shooting is starting");
            manager.firing = true;

        
    }
}
