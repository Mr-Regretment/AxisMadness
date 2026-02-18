using UnityEngine;

public class FinishlineHandler : MonoBehaviour
{
    [SerializeField] private int NextLevelIndex;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera _camera;
    private PlayerMovement playerMovement;
    private Vector3 targetPosition;
    private bool hasTransitioned = false;

    void Start()
    {
        if (UIHandler.Instance == null)
        {
            GameObject uiObj = new GameObject("UIHandler");
            UIHandler uiHandler = uiObj.AddComponent<UIHandler>();
        }

        if (player == null)
        {
            return;
        }
        if (_camera == null)
        {
            return;
        }

        playerMovement = player.GetComponent<PlayerMovement>();
        targetPosition = _camera.transform.position + Vector3.down * 40;
    }

    void Update()
    {
        if (hasTransitioned) return;
    
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.up);
    
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f))
            return;
    
        if (hitInfo.transform == null)
            return;
    
        if (!hitInfo.transform.CompareTag("Player"))
            return;
    
    
        if (UIHandler.Instance == null)
        {
            return;
        }
    
        hasTransitioned = true;
        playerMovement.ShouldMove = false;
    
        Vector3 targetPos = new Vector3(
            player.transform.position.x, 
            _camera.transform.position.y - 70,
            player.transform.position.z
        );

    
        UIHandler.Instance.CameraTransitionToScene(
            _camera,
            targetPos,
            Quaternion.Euler(_camera.transform.eulerAngles.x, _camera.transform.eulerAngles.y, _camera.transform.eulerAngles.z),
            NextLevelIndex,4f);
    }
}