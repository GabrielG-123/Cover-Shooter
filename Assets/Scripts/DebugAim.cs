using UnityEngine;

public class DebugAim : MonoBehaviour
{
    public Transform playerBody;
    private bool hasSnapped = false;

    void Update()
    {
        // 1. Detect the initial press
        if (Input.GetMouseButton(1) && !hasSnapped)
        {
            DoTheSnap();
            hasSnapped = true; // <-- THE MISSING LOCK
        }

        // 2. Reset when the player lets go
        if (Input.GetMouseButtonUp(1))
        {
            hasSnapped = false;
            Debug.Log("Aim Released - Snap Reset");
        }
    }

    void DoTheSnap()
    {
        // Use the Main Camera (FreeLook perspective) to find the world target
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        // Use a LayerMask to avoid hitting the player's own collider
        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, layerMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        // VISUAL DEBUG
        Debug.DrawLine(ray.origin, targetPoint, Color.cyan, 2f);

        // Calculate Direction
        Vector3 worldTargetDir = (targetPoint - playerBody.position).normalized;
        worldTargetDir.y = 0;

        if (worldTargetDir != Vector3.zero)
        {
            playerBody.rotation = Quaternion.LookRotation(worldTargetDir);
            Debug.Log("SUCCESS: Snapped to " + targetPoint);
        }
    }
}