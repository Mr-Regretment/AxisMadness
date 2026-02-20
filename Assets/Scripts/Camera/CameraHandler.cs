using System.Collections;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    #region  Variables
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private GameObject player;
    [SerializeField] private Quaternion targetCameraRotation;
    private Quaternion targetPlayerRotation;
    public Vector3 targetPosition;
    [SerializeField] private bool isRotating = false;
    private Vector3 _targetRoundedPos;
    private bool initialMoveDone = false;
    private bool automaticCameraReposition = true;
    
    private int _visibleLayer;
    private int _hiddenLayer;
    private int _fadingLayer;
    
    private const float CAMERA_Y_OFFSET = 7.5f;
    #endregion
    
    void Start()
    {
        _targetRoundedPos = player.transform.position;
        targetCameraRotation = transform.rotation;
        targetPlayerRotation = player.transform.rotation;
        
        targetPosition = transform.position + Vector3.down * CAMERA_Y_OFFSET;
        
        _visibleLayer = LayerMask.NameToLayer("VisibleAxis");
        _hiddenLayer = LayerMask.NameToLayer("HiddenAxis");
        _fadingLayer = LayerMask.NameToLayer("FadingAxis");
    }
    private void ToggleAxisLayers()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == _visibleLayer)
                StartCoroutine(FadeObject(obj, 1f, 0f, _hiddenLayer));
            else if (obj.layer == _hiddenLayer)
                StartCoroutine(FadeObject(obj, 0f, 1f, _visibleLayer));
        }
    }
    private IEnumerator FadeObject(GameObject obj, float startAlpha, float endAlpha, int targetLayer)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer == null)
        {
            obj.layer = targetLayer;
            yield break;
        }

        // Get ALL materials on the renderer
        Material[] materials = renderer.materials;

        bool hasAlpha = false;
        foreach (Material mat in materials)
            if (mat.HasProperty("_Alpha"))
                hasAlpha = true;

        if (!hasAlpha)
        {
            renderer.enabled = endAlpha > 0f;
            obj.layer = targetLayer;
            yield break;
        }

        obj.layer = _fadingLayer;

        foreach (Material mat in materials)
            if (mat.HasProperty("_Alpha"))
                mat.SetFloat("_Alpha", startAlpha);

        float timer = 0f;
        float duration = 0.5f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            foreach (Material mat in materials)
                if (mat.HasProperty("_Alpha"))
                    mat.SetFloat("_Alpha", alpha);
            yield return null;
        }

        foreach (Material mat in materials)
            if (mat.HasProperty("_Alpha"))
                mat.SetFloat("_Alpha", endAlpha);

        obj.layer = targetLayer;
    }
    private void Rotation()
    {
        if (isRotating)
            return;

        PlayerCamera playerCamera = player.GetComponent<PlayerCamera>();

        if (playerCamera == null)
            return;

    
        if (!playerCamera.StandingOverRotatePad())
            return;

        if (!Input.GetKey(KeyCode.LeftShift))
            return;

        if (playerCamera.tokenCount <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !Input.GetKeyDown(KeyCode.Q))
        {
            playerCamera.tokenCount--;
            isRotating = true;
            ToggleAxisLayers();
            
            float x = (float)System.Math.Round(player.transform.position.x, 1);
            float z = (float)System.Math.Round(player.transform.position.z, 1);

            _targetRoundedPos = new Vector3(x, player.transform.position.y, z);
            targetPosition = player.transform.position;

            targetCameraRotation *= Quaternion.Euler(0, -90f, 0);
            targetPlayerRotation *= Quaternion.Euler(0, -90f, 0);
        }

        if (Input.GetKeyDown(KeyCode.Q) && !Input.GetKeyDown(KeyCode.E))
        {
            playerCamera.tokenCount--;
            isRotating = true;
            ToggleAxisLayers();

            float x = (float)System.Math.Round(player.transform.position.x, 1);
            float z = (float)System.Math.Round(player.transform.position.z, 1);

            _targetRoundedPos = new Vector3(x, player.transform.position.y, z);
            targetPosition = player.transform.position;

            targetCameraRotation *= Quaternion.Euler(0, 90f, 0);
            targetPlayerRotation *= Quaternion.Euler(0, 90f, 0);
        }
    }

    void Update()
    {
        bool isTransitioning = (UIHandler.Instance != null && UIHandler.Instance.IsTransitioning);

        if (isTransitioning)
            return;

        if (!initialMoveDone && Vector3.Distance(transform.position, targetPosition) < 0.5f)
            initialMoveDone = true;

        if (Input.GetKeyDown(KeyCode.R) || (player.GetComponent<CharacterOffScreen>().isOffscreen && automaticCameraReposition && initialMoveDone))
        {
            targetPosition = new Vector3(player.transform.position.x, targetPosition.y, player.transform.position.z);
        }

        if (isRotating)
        {
            player.GetComponent<PlayerMovement>().ShouldMove = false;

            float cameraDiff = Quaternion.Angle(transform.rotation, targetCameraRotation);
            float playerDiff = Quaternion.Angle(player.transform.rotation, targetPlayerRotation);

            if (cameraDiff < 1f && playerDiff < 1f)
            {
                isRotating = false;
                ResetToggle();
                player.GetComponent<PlayerMovement>().ShouldMove = true;
            }
        }

        Rotation();
        transform.rotation = Quaternion.Slerp(transform.rotation, targetCameraRotation, Time.deltaTime * 5f);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetPlayerRotation, Time.deltaTime * rotationSpeed);
        CameraReposition();
    }

    private bool hasToggledShouldMove = false;

    void ToggleShouldMove(bool shouldMoveSet)
    {
        if (!hasToggledShouldMove)
        {
            player.GetComponent<PlayerMovement>().ShouldMove = shouldMoveSet;
            hasToggledShouldMove = true;
        }
    }

    public IEnumerator CameraMove(Vector3 newPosition, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        automaticCameraReposition = false;

        Vector3 startPosition = transform.position;
        targetPosition = newPosition;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosition) < 2f);

        yield return new WaitForSeconds(duration);

        targetPosition = startPosition;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosition) < 2f);

        automaticCameraReposition = true;
    }

    void ResetToggle()
    {
        hasToggledShouldMove = false;
    }

    public bool OverrideShouldMove { get; set; } = false;

    void CameraReposition()
    {
        transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * 2f);
    
        if (OverrideShouldMove)
            return;
    }
    
    public bool IsRotatingAnimation()
    {
        return isRotating;
    }
}