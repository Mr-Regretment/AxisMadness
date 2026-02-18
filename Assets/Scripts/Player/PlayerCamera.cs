using UnityEngine;

public class PlayerCamera : PlayerHandler
{
    [SerializeField] private CameraHandler cameraHandler;
    private Renderer _renderer;
    private PlayerMovement _playerMovement;
    [SerializeField] private ModelCyclingAnimation _modelCycling;
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        
        _playerMovement = GetComponent<PlayerMovement>();
        
        if (cameraHandler == null)
            cameraHandler = FindFirstObjectByType<CameraHandler>();
    }
    
    private void Update()
    {
        if (cameraHandler == null)
            return;
        

        float distanceToPlayer = Vector3.Distance(cameraHandler.transform.position, transform.position);
    }

    public bool StandingOverRotatePad()
    {
        if (rigidbody == null)
            return false;

        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
    
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f))
            return false;
    
        if (hitInfo.transform == null)
            return false;

        return hitInfo.transform.CompareTag("RotatePad");
    }
    

    public bool OnScreen() => _renderer.isVisible;
    public bool OffScreen() => !_renderer.isVisible;
}