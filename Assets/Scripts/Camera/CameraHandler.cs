using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] PlayerHandler _playerHandler;

    private PlayerMovement _playerMovement;
    private PlayerCamera _playerCamera;
    private Rigidbody _playerRigidbody;
    private CharacterOffScreen _characterOffScreen;

    public Quaternion targetCameraRotation;
    private Quaternion targetPlayerRotation;
    public Vector3 targetPosition;

    private bool _onRotatePad = false;
    private bool isRotating = false;

    private bool _isRepositioning = false;
    private bool automaticCameraReposition = true;
    private bool initialMoveDone = false;
    private float _cameraMoveLockTimer = 0f;

    public bool OverrideShouldMove { get; set; } = false;

    public System.Action OnCameraFocus;

    private const float CAMERA_Y_OFFSET = 7.5f;

    private RigidbodyConstraints _preRotateConstraints;
    
    private Quaternion _confirmedCameraRotation;
    private Quaternion _confirmedPlayerRotation;
    private Quaternion _previewCameraRotation;
    private Quaternion _previewPlayerRotation;

    private bool _pendingMovementRestore = false;

    void Start()
    {
        _playerMovement = player.GetComponent<PlayerMovement>();
        _playerCamera = player.GetComponent<PlayerCamera>();
        _playerRigidbody = player.GetComponent<Rigidbody>();
        _characterOffScreen = player.GetComponent<CharacterOffScreen>();
        
        if(_playerHandler == null)
            _playerHandler = player.GetComponent<PlayerHandler>();

        targetCameraRotation = transform.rotation;
        targetPlayerRotation = player.transform.rotation;
        targetPosition = transform.position + Vector3.down * CAMERA_Y_OFFSET;

        _confirmedCameraRotation = targetCameraRotation;
        _confirmedPlayerRotation = targetPlayerRotation;
        _previewCameraRotation = targetCameraRotation;
        _previewPlayerRotation = targetPlayerRotation;
    }

    void Update()
    {
        HandleRotation();
        HandleReposition();
        ApplyTransforms();
    }

    private void HandleRotation()
    {
        if (_playerCamera == null)
            return;

        bool onPad = _playerCamera.StandingOverRotatePad();

        if (onPad && !_onRotatePad)
        {
            _onRotatePad = true;
            _playerMovement.ShouldMoveHorizontal = false;

            Vector3 vel = _playerRigidbody.linearVelocity;
            if (Mathf.Abs(vel.x) > Mathf.Abs(vel.z))
                _preRotateConstraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            else
                _preRotateConstraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;

            _confirmedCameraRotation = targetCameraRotation;
            _confirmedPlayerRotation = targetPlayerRotation;
            _previewCameraRotation = targetCameraRotation;
            _previewPlayerRotation = targetPlayerRotation;

            Collider padCollider = _playerCamera.CurrentRotatePad().GetComponent<Collider>();
            float padTopY = padCollider.bounds.max.y;
            float playerHalfHeight = player.GetComponent<Collider>().bounds.extents.y;

            Vector3 padCenter = padCollider.bounds.center;
            StartCoroutine(SmoothSnapToPad(new Vector3(padCenter.x, padTopY + playerHalfHeight, padCenter.z)));
        }

        if (!onPad && _onRotatePad)
        {
            _onRotatePad = false;
            _playerMovement.ShouldMoveHorizontal = true;
            _pendingMovementRestore = false;

            targetCameraRotation = _confirmedCameraRotation;
            targetPlayerRotation = _confirmedPlayerRotation;
            StartCoroutine(LockAxisDuringReset());
        }

        if (Input.GetKeyDown(KeyCode.R))
            FocusOnPlayer();
        
        if (!_onRotatePad)
            return;

        if (_playerCamera.tokenCount <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _previewCameraRotation *= Quaternion.Euler(0, 90f, 0);
            _previewPlayerRotation *= Quaternion.Euler(0, 90f, 0);
            targetCameraRotation = _previewCameraRotation;
            targetPlayerRotation = _previewPlayerRotation;
            StartRotation();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _previewCameraRotation *= Quaternion.Euler(0, -90f, 0);
            _previewPlayerRotation *= Quaternion.Euler(0, -90f, 0);
            targetCameraRotation = _previewCameraRotation;
            targetPlayerRotation = _previewPlayerRotation;
            StartRotation();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _playerCamera.tokenCount--;
            CameraMove(player.transform.position, false);
            _confirmedCameraRotation = _previewCameraRotation;
            _confirmedPlayerRotation = _previewPlayerRotation;
            _pendingMovementRestore = true;
            StartRotation();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _previewCameraRotation = _confirmedCameraRotation;
            _previewPlayerRotation = _confirmedPlayerRotation;
            targetCameraRotation = _confirmedCameraRotation;
            targetPlayerRotation = _confirmedPlayerRotation;
            StartRotation();
        }

        if (isRotating)
        {
            float camDiff = Quaternion.Angle(transform.rotation, targetCameraRotation);
            float playerDiff = Quaternion.Angle(player.transform.rotation, targetPlayerRotation);

            if (camDiff < 1f && playerDiff < 1f)
            {
                isRotating = false;
                if (_pendingMovementRestore)
                {
                    _pendingMovementRestore = false;
                    _playerMovement.ShouldMoveHorizontal = true;
                }
            }
        }
    }
    
    private IEnumerator LockAxisDuringReset()
    {
        _playerRigidbody.constraints = _preRotateConstraints;

        yield return new WaitUntil(() =>
            Quaternion.Angle(transform.rotation, targetCameraRotation) < 1f &&
            Quaternion.Angle(player.transform.rotation, targetPlayerRotation) < 1f
        );

        _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
    
    private IEnumerator SmoothSnapToPad(Vector3 target)
    {
        _playerRigidbody.linearVelocity = Vector3.zero;
        _playerRigidbody.isKinematic = true;

        while (Vector3.Distance(player.transform.position, target) > 0.01f)
        {
            player.transform.position = Vector3.Lerp(
                player.transform.position,
                target,
                Time.deltaTime * 12f
            );
            yield return null;
        }

        player.transform.position = target;
        _playerRigidbody.isKinematic = false;
    }

    public bool IsRotatingAnimation()
    {
        return isRotating;
    }

    private void StartRotation()
    {
        isRotating = true;
        OnCameraFocus?.Invoke();
    }

    private void HandleReposition()
    {
        if (!initialMoveDone && Vector3.Distance(transform.position, targetPosition) < 0.5f)
            initialMoveDone = true;

        if (_playerHandler.isDead)
            return;

        if (_cameraMoveLockTimer > 0f)
        {
            _cameraMoveLockTimer -= Time.deltaTime;
            return;
        }

        if (_characterOffScreen != null &&
            _characterOffScreen.isOffscreen &&
            automaticCameraReposition &&
            initialMoveDone)
        {
            Vector3 newTarget = new Vector3(
                player.transform.position.x,
                transform.position.y,
                player.transform.position.z
            );

            if (Vector3.Distance(transform.position, newTarget) > 1f && !_isRepositioning)
                _isRepositioning = true;

            targetPosition = newTarget;
        }

        if (_isRepositioning)
        {
            if (Vector3.Distance(transform.position, targetPosition) < 1f)
                _isRepositioning = false;
        }

        if (initialMoveDone && automaticCameraReposition)
        {
            float y = Mathf.Lerp(transform.position.y, player.transform.position.y, Time.deltaTime * 25f);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }

    public void FocusOnPlayer(float lockDuration = 0.75f)
    {
        if (player == null) return;

        CameraMove(new Vector3(
            player.transform.position.x,
            transform.position.y,
            player.transform.position.z
        ), false,lockDuration);

        OnCameraFocus?.Invoke();
    }

    private IEnumerator LockMovementRoutine(float duration)
    {
        if (_playerMovement != null)
        {
            _playerMovement.SetMovementEnabled(false);
            yield return new WaitForSeconds(duration);
            _playerMovement.SetMovementEnabled(true);
        }
    }

    private void ApplyTransforms()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetCameraRotation, Time.deltaTime * 5f);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetPlayerRotation, Time.deltaTime * rotationSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 4f);
    }

    public void CameraMove(Vector3 tarPos,bool locksMovement,float lockDuration = 2f)
    {
        targetPosition = tarPos;
        _cameraMoveLockTimer = lockDuration;
        if(locksMovement)
            StartCoroutine(LockMovementRoutine(lockDuration));
    }

    public void SetPlayerMovement(bool enabled)
    {
        OverrideShouldMove = !enabled;

        if (_playerMovement != null)
            _playerMovement.SetMovementEnabled(enabled);
    }
}