using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private GameObject player;
    [SerializeField] private Quaternion targetCameraRotation;
    private Quaternion targetPlayerRotation;
    public Vector3 targetPosition;
    [SerializeField] private bool isRotating = false;
    private Vector3 _targetRoundedPos;
    private bool initialMoveDone = false;
    
    private const float CAMERA_Y_OFFSET = 7.5f;
    
    void Start()
    {
        _targetRoundedPos = player.transform.position;
        targetCameraRotation = transform.rotation;
        targetPlayerRotation = player.transform.rotation;
    
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        targetPosition = transform.position + Vector3.down * CAMERA_Y_OFFSET;
    }

    private void Rotation()
    {
        PlayerCamera playerCamera = player.GetComponent<PlayerCamera>();

        if (playerCamera == null)
            return;
    
        if (!playerCamera.StandingOverRotatePad())
            return;

        if (!Input.GetKey(KeyCode.LeftShift))
            return;

        if (playerCamera.tokenCount <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            playerCamera.tokenCount--;
            isRotating = true;
            
            float x = (float)System.Math.Round(player.transform.position.x, 1);
            float z = (float)System.Math.Round(player.transform.position.z, 1);

            _targetRoundedPos = new Vector3(x, player.transform.position.y, z);
            targetPosition = player.transform.position;

            targetCameraRotation *= Quaternion.Euler(0, -90f, 0);
            targetPlayerRotation *= Quaternion.Euler(0, -90f, 0);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerCamera.tokenCount--;
            isRotating = true;
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

        if (Input.GetKeyDown(KeyCode.R) || player.GetComponent<CharacterOffScreen>().isOffscreen)
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
    
        if (Vector3.Distance(transform.position, targetPosition) > 5f)
        {
            ResetToggle();
            player.GetComponent<PlayerMovement>().ShouldMove = false;
        }
        else
        {
            ToggleShouldMove(true);
        }
    }
    
    public bool IsRotatingAnimation()
    {
        return isRotating;
    }
}