using Unity.Cinemachine;
using UnityEngine;

public class FreelookCamPivot : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] Transform spineFollow;
    public Vector3 initialPosition;
    public float crouchedPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.localPosition;
        crouchedPosition = spineFollow.position.y - 0.85f;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (animator.GetBool("isCrouched"))
        {
            transform.position = new Vector3(spineFollow.position.x, crouchedPosition, spineFollow.position.z);



        }
        else transform.localPosition = initialPosition;

    }
}
