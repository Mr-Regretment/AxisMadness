using UnityEngine;

public class FinishlineHandler : MonoBehaviour
{
    [SerializeField] private int NextLevelIndex;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera _camera;

    private PlayerMovement playerMovement;
    private UIHandler _uiHandler;
    private bool hasTransitioned = false;

    void Start()
    {
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();

        _uiHandler = FindFirstObjectByType<UIHandler>();
    }

    void Update()
    {
        if (hasTransitioned) return;
        if (player == null || _camera == null) return;

        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.red);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1.5f))
            return;

        if (!hit.transform.CompareTag("Player")) return;

        hasTransitioned = true;

        if (playerMovement != null)
            playerMovement.ShouldMove = false;

        if (_uiHandler == null)
            _uiHandler = FindFirstObjectByType<UIHandler>();

        if (_uiHandler == null)
        {
            GameObject uiHandlerObj = new GameObject("UIHandler");
            _uiHandler = uiHandlerObj.AddComponent<UIHandler>();
        }

        Vector3 targetPos = new Vector3(
            player.transform.position.x,
            _camera.transform.position.y - 70,
            player.transform.position.z
        );

        _uiHandler.CameraTransitionToScene(
            _camera,
            targetPos,
            _camera.transform.rotation,
            NextLevelIndex,
            4f
        );
    }
}