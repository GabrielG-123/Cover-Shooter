using UnityEngine;

public class RadiusScript : MonoBehaviour
{

    Renderer renderComponent;
    float radius;
    //float depth;
    Vector3 centerPos;
    Vector3 radiusPos;

    [SerializeField] Transform cameraObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderComponent = GetComponent<Renderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
     //   depth = renderComponent.bounds.extents.z;

       // Debug.Log("The radius of sphere is" + radius);
       // Debug.Log("The depth of the sphere is: " + depth);
    }
}
