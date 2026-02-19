using UnityEngine;

public class CharacterOffScreen : MonoBehaviour
{
    private Camera mainCamera;
    public bool isOffscreen { get; private set; }
    [SerializeField] private float worldUnitMargin = 2.5f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);

        float depth = screenPoint.z;
        Vector3 centerWorld = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, depth));
        Vector3 offsetWorld = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, depth));
        float viewportUnitsPerWorldUnit = 0.5f / Vector3.Distance(centerWorld, offsetWorld) * worldUnitMargin;

        isOffscreen = screenPoint.x < viewportUnitsPerWorldUnit || screenPoint.x > 1 - viewportUnitsPerWorldUnit ||
                      screenPoint.y < viewportUnitsPerWorldUnit || screenPoint.y > 1 - viewportUnitsPerWorldUnit ||
                      screenPoint.z < 0;
    }
}