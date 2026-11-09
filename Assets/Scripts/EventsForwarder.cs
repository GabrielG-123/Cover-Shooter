using UnityEngine;

public class EventsForwarder : MonoBehaviour
{

    public PlayerController playerController;   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void vaultingEvent() { 
    
        playerController.vaultingEvent();




    }


    public void vaultingStartEvent() {



        playerController.vaultingStartEvent();
    
    
    }
}
