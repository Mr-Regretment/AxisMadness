using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : PlayerHandler
{
    [SerializeField] private CameraHandler cameraHandler;
    private Renderer _renderer;
    private PlayerMovement _playerMovement;
    
    [SerializeField] private GameObject leftRotateArrow;
    [SerializeField] private GameObject rightRotateArrow;
    [SerializeField] private GameObject acceptNewRotationText;
    private Vector3 acceptedCameraRotationTargetPos;
    private Vector3 acceptedCameraRotationStartingPos;
    private Image leftImage;
    private Image rightImage;
    private TextMeshProUGUI leftText;
    private TextMeshProUGUI rightText;
    [SerializeField] protected GameObject guiTokenCount;

    private GameObject _currentRotatePad;

    void Start()
    {
        leftImage = leftRotateArrow.GetComponent<Image>();
        rightImage = rightRotateArrow.GetComponent<Image>();
        if (acceptNewRotationText != null)
            acceptedCameraRotationStartingPos = acceptNewRotationText.transform.position;
        leftText = leftRotateArrow.GetComponentInChildren<TextMeshProUGUI>();
        rightText = rightRotateArrow.GetComponentInChildren<TextMeshProUGUI>();
    
        Color leftImageColour = leftImage.color;
        Color rightImageColor = rightImage.color;
        leftImageColour.a = 0;
        rightImageColor.a = 0;
        
        _renderer = GetComponent<Renderer>();
        _playerMovement = GetComponent<PlayerMovement>();
        
        if (cameraHandler == null)
            cameraHandler = FindFirstObjectByType<CameraHandler>();
    }

    private float targetAlpha = 0f;
    private void Update()
    {
        if (acceptNewRotationText != null && StandingOverRotatePad())
        {
            if (cameraHandler.HasUnconfirmedRotation)
                acceptedCameraRotationTargetPos = acceptedCameraRotationStartingPos + (Vector3.down * 65);
            else
                acceptedCameraRotationTargetPos = acceptedCameraRotationStartingPos;
        }
        else
        {
            acceptedCameraRotationTargetPos = acceptedCameraRotationStartingPos;
        }

        acceptNewRotationText.transform.position = Vector3.Lerp(acceptNewRotationText.transform.position,acceptedCameraRotationTargetPos, Time.deltaTime * 8f);
        
        TextMeshProUGUI tokenCountGui = guiTokenCount.GetComponent<TextMeshProUGUI>();
        tokenCountGui.text = tokenCount.ToString();
        Color leftImageColour = leftImage.color;
        Color rightImageColor = rightImage.color;
        Color leftTextColour = leftText.color;
        Color rightTextColour = rightText.color;
        
        targetAlpha = Input.GetKey(KeyCode.LeftShift) && StandingOverRotatePad() ? 1 : 0;
            
        leftImageColour.a = Mathf.Lerp(leftImageColour.a, targetAlpha, Time.deltaTime * 10f);
        rightImageColor.a = Mathf.Lerp(rightTextColour.a, targetAlpha, Time.deltaTime * 10f);
        leftTextColour.a = Mathf.Lerp(leftImageColour.a, targetAlpha, Time.deltaTime * 10f);
        rightTextColour.a = Mathf.Lerp(rightTextColour.a, targetAlpha, Time.deltaTime * 10f);
        
        leftImage.color = leftImageColour;   
        rightImage.color = rightImageColor;
        leftText.color = leftTextColour;
        rightText.color = rightTextColour;
    }

    public bool StandingOverRotatePad()
    {
        if (rigidbody == null)
            return false;

        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f))
        {
            _currentRotatePad = null;
            return false;
        }

        if (hitInfo.transform == null)
        {
            _currentRotatePad = null;
            return false;
        }

        if (hitInfo.transform.CompareTag("RotatePad"))
        {
            _currentRotatePad = hitInfo.transform.gameObject;
            return true;
        }

        _currentRotatePad = null;
        return false;
    }

    public GameObject CurrentRotatePad() => _currentRotatePad;

    public bool OnScreen() => _renderer.isVisible;
    public bool OffScreen() => !_renderer.isVisible;
}