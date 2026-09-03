using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    [SerializeField] private int targetFPS = 60;

    private void Awake()
    {
        // Must be set to 0, otherwise targetFrameRate is ignored
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }

    private void Update()
    {
        // Allows adjusting the slider dynamically in the Inspector while testing
        if (Application.targetFrameRate != targetFPS)
        {
            Application.targetFrameRate = targetFPS;
        }
    }
}