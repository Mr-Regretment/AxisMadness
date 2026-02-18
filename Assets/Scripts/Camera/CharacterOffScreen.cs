using UnityEngine;

public class CharacterOffScreen : MonoBehaviour
{
    private Camera mainCamera;
    public bool isOffscreen { get; private set; }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        
        isOffscreen = screenPoint.x < 0 || screenPoint.x > 1 || 
                      screenPoint.y < 0 || screenPoint.y > 1 || 
                      screenPoint.z < 0;
    }
}