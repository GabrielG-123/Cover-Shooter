using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;

public abstract class State
{
    protected Animator Animator;
    protected StateManager _manager;
    RaycastHit hit;
    LayerMask layerMask;

    public State(StateManager stateManager)
    {
        

            _manager = stateManager;
        //layerMask = LayerMask.GetMask("Player");


    }


    public virtual void OnEnter() { 
    
    
    
    }
    public abstract State RunCurrentState();


    public virtual void OnExit() { }


    public virtual bool visionCone() {




       // Debug.DrawRay(_manager.agent.transform.position + new Vector3(0, 1.0f, 0), _manager.direction * 10f, Color.red);
        if (_manager.distanceFromPlayer <= 15.0f)
        {


            //Debug.DrawRay(_manager.head.position, _manager.direction * 20f, Color.magenta);
            if (Physics.Raycast(_manager.head.position, _manager.direction, out hit, 20f))
            {

                //Debug.Log("The collider I am hitting is: " + hit.collider.name);



                if (hit.transform.CompareTag("Player")) {

                   // Debug.Log("Hitting the player");
                
                }

                    if (_manager.coneAngle >= 0.5 && hit.transform.CompareTag("Player"))
                    {
                    Debug.Log("Yup, that happened");

                        if (_manager.alertMeter >= 0 && _manager.alertMeter <= 100)
                        {
                            if (_manager.distanceFromPlayer <= 5.0f) _manager.alertMeter += 50.0f;
                            if (_manager.distanceFromPlayer > 5.0f && _manager.distanceFromPlayer <= 10.0f) _manager.alertMeter += 15.0f;
                            if (_manager.distanceFromPlayer > 10.0f && _manager.distanceFromPlayer <= 20.0f) _manager.alertMeter += 8.5f;
                        }

                        if (_manager.alertMeter < 0) _manager.alertMeter = 0;
                        if (_manager.alertMeter > 100) _manager.alertMeter = 100;
                        return true;


                    }


                





            }

           
            






        }

        else if (_manager.alertMeter > 0)
        {
           // Debug.Log("not hitting the player");
            _manager.alertMeter -= 5.0f;


        }

        return false;



    }

    public virtual void promximityAlert()
    {

        Collider[] colliders = new Collider[50];

        int numColliders = Physics.OverlapSphereNonAlloc(_manager.transform.position, 20.0f, colliders, _manager.coverScript.searchLayer);

        for (int i = 0; i < numColliders; i++)
        {
            Collider collider = colliders[i];


            if (collider.CompareTag("Enemy") && (collider.transform.root.GetComponent<StateManager>().isAlerted == true) && (collider != _manager.coverScript.hitboxCollider))
            {
                if (!_manager.isAlerted)
                {
                    Debug.Log("I need to be alerted" + collider.transform.root.name);
                    _manager.alertMeter = 100;
                    _manager.isAlerted = true;
                }
                // Debug.Log("The component is null for the enemy: " + collider.transform.root.name);
            }
            //if (collider.CompareTag("Enemy") && collider != _manager.coverScript.hitboxCollider && collider.transform.root.GetComponent<StateManager>().isAlerted) { 
            
            
            //    Debug.Log("The name of the alerted enemy is: " + collider.transform.root.name);
            //}



            //if (collider.CompareTag("Enemy") && collider.GetComponentInParent<StateManager>().isAlerted && !_manager.isAlerted
            //    && collider != _manager.coverScript.hitboxCollider)
            //{
            //    Debug.Log("The name of the alerted enemy is: " + collider.transform.root.name);
            //    _manager.alertMeter = 100;

            //}




        }

    }
}



    

