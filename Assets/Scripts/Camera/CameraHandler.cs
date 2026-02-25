using System.Collections;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    #region Variables
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private GameObject player;
    [SerializeField] private Quaternion targetCameraRotation;
    private Quaternion targetPlayerRotation;
    public Vector3 targetPosition;
    [SerializeField] private bool isRotating = false;
    private Vector3 _targetRoundedPos;
    private bool initialMoveDone = false;
    private bool automaticCameraReposition = true;

    private const float CAMERA_Y_OFFSET = 7.5f;

    private Quaternion _confirmedCameraRotation;
    private Quaternion _confirmedPlayerRotation;
    private Quaternion _previewCameraRotation;
    private Quaternion _previewPlayerRotation;
    private bool _onRotatePad = false;
    private bool _snapComplete = false;

    private Rigidbody _playerRigidbody;
    private PlayerMovement _playerMovement;
    private PlayerCamera _playerCamera;
    private CharacterOffScreen _characterOffScreen;

    private bool _isRepositioning = false;
    private bool _rotationConfirmed = false;

    [SerializeField] private GlitchEffect glitchEffect;
    #endregion

    void Start()
    {
        _playerRigidbody = player.GetComponent<Rigidbody>();
        _playerMovement = player.GetComponent<PlayerMovement>();
        _playerCamera = player.GetComponent<PlayerCamera>();
        _characterOffScreen = player.GetComponent<CharacterOffScreen>();

        _targetRoundedPos = player.transform.position;
        targetCameraRotation = transform.rotation;
        targetPlayerRotation = player.transform.rotation;
        targetPosition = transform.position + Vector3.down * CAMERA_Y_OFFSET;
    }

    private void Rotation()
    {
        if (isRotating)
            return;

        if (_playerCamera == null)
            return;

        bool onPad = _playerCamera.StandingOverRotatePad();

        if (onPad && !_onRotatePad)
        {
            _onRotatePad = true;
            _snapComplete = false;
            if (!OverrideShouldMove)
                _playerMovement.ShouldMove = false;

            _confirmedCameraRotation = targetCameraRotation;
            _confirmedPlayerRotation = targetPlayerRotation;
            _previewCameraRotation = targetCameraRotation;
            _previewPlayerRotation = targetPlayerRotation;

            float x = Mathf.Round(player.transform.position.x);
            float z = Mathf.Round(player.transform.position.z);
            float padTopY = _playerCamera.CurrentRotatePad().GetComponent<Collider>().bounds.max.y;
            StartCoroutine(SnapPlayerToPad(new Vector3(x, padTopY, z), () => _snapComplete = true));
        }

        if (!onPad && _onRotatePad)
        {
            _onRotatePad = false;
            _snapComplete = false;
            _rotationConfirmed = false;
            targetCameraRotation = _confirmedCameraRotation;
            targetPlayerRotation = _confirmedPlayerRotation;
            if (!OverrideShouldMove)
                _playerMovement.ShouldMove = true;
            return;
        }

        if (!onPad || !_snapComplete)
            return;

        if (!_rotationConfirmed && !OverrideShouldMove)
            _playerMovement.ShouldMove = false;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (_playerCamera.tokenCount > 0)
            {
                _playerCamera.tokenCount--;
                isRotating = true;
                _snapComplete = false;
                _rotationConfirmed = true;
                _confirmedCameraRotation = _previewCameraRotation;
                _confirmedPlayerRotation = _previewPlayerRotation;
                targetPosition = player.transform.position;
                glitchEffect?.TriggerGlitch();
                if (!OverrideShouldMove)
                    _playerMovement.ShouldMove = true;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _previewCameraRotation = _confirmedCameraRotation;
            _previewPlayerRotation = _confirmedPlayerRotation;
            targetCameraRotation = _confirmedCameraRotation;
            targetPlayerRotation = _confirmedPlayerRotation;
            glitchEffect?.TriggerGlitch();
            return;
        }

        if (!Input.GetKey(KeyCode.LeftShift) || _playerCamera.tokenCount <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.Q) && !Input.GetKeyDown(KeyCode.E))
        {
            _previewCameraRotation *= Quaternion.Euler(0, 90f, 0);
            _previewPlayerRotation *= Quaternion.Euler(0, 90f, 0);
            targetCameraRotation = _previewCameraRotation;
            targetPlayerRotation = _previewPlayerRotation;
            glitchEffect?.TriggerGlitch();
            
        }

        if (Input.GetKeyDown(KeyCode.E) && !Input.GetKeyDown(KeyCode.Q))
        {
            _previewCameraRotation *= Quaternion.Euler(0, -90f, 0);
            _previewPlayerRotation *= Quaternion.Euler(0, -90f, 0);
            targetCameraRotation = _previewCameraRotation;
            targetPlayerRotation = _previewPlayerRotation;
            glitchEffect?.TriggerGlitch();
        }
    }

    private IEnumerator SnapPlayerToPad(Vector3 snapTarget, System.Action onComplete)
    {
        _playerRigidbody.linearVelocity = Vector3.zero;
        _playerRigidbody.isKinematic = true;
        player.transform.position = snapTarget;

        while (Vector3.Distance(player.transform.position, snapTarget) > 0.01f)
        {
            player.transform.position = Vector3.Lerp(
                player.transform.position,
                snapTarget,
                Time.deltaTime * 12f
            );
            yield return null;
        }

        player.transform.position = snapTarget;
        _playerRigidbody.isKinematic = false;
        onComplete?.Invoke();
    }

    void Update()
    {
        bool isTransitioning = (UIHandler.Instance != null && UIHandler.Instance.IsTransitioning);

        if (isTransitioning)
            return;

        if (!initialMoveDone && Vector3.Distance(transform.position, targetPosition) < 0.5f)
            initialMoveDone = true;

        if (_characterOffScreen.isOffscreen && automaticCameraReposition && initialMoveDone)
        {
            Vector3 newTarget = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

            if (Vector3.Distance(transform.position, newTarget) > 1f && !_isRepositioning)
            {
                _isRepositioning = true;
                if (!OverrideShouldMove)
                    _playerMovement.ShouldMove = false;
            }

            targetPosition = newTarget;
        }

        if (_isRepositioning)
        {
            bool isAirborne = Mathf.Abs(_playerRigidbody.linearVelocity.y) > 0.1f;

            if (Vector3.Distance(transform.position, targetPosition) < 1f || isAirborne)
            {
                _isRepositioning = false;
                if (!OverrideShouldMove)
                    _playerMovement.ShouldMove = true;
            }
        }

        if (initialMoveDone && automaticCameraReposition)
        {
            float yPos = Mathf.Lerp(transform.position.y, player.transform.position.y, Time.deltaTime * 25f);
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.R))
            targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        if (isRotating)
        {
            if (!OverrideShouldMove)
                _playerMovement.ShouldMove = false;

            float cameraDiff = Quaternion.Angle(transform.rotation, targetCameraRotation);
            float playerDiff = Quaternion.Angle(player.transform.rotation, targetPlayerRotation);

            if (cameraDiff < 1f && playerDiff < 1f)
            {
                isRotating = false;
                if (!OverrideShouldMove)
                    _playerMovement.ShouldMove = true;
            }
        }

        Rotation();
        transform.rotation = Quaternion.Slerp(transform.rotation, targetCameraRotation, Time.deltaTime * 5f);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetPlayerRotation, Time.deltaTime * rotationSpeed);
        transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * 2f);
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

    public bool OverrideShouldMove { get; set; } = false;

    public bool IsRotatingAnimation()
    {
        return isRotating;
    }
}