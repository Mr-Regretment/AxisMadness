using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject pressedVisual;
    [SerializeField] private GameObject normalVisual;
    [SerializeField] private float detectionRange = 1.2f;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private float soundVolume = 1f;
    
    [Header("Select Object")]
    [SerializeField] private bool selectObject;
    [SerializeField] private GameObject _selectObject;

    private AudioSource _sfxSource;
    private bool isPressed;
    private const string PLAYER_TAG = "Player";
    private const string PHYSICS_TAG = "PhysicsObject";

    private void Start()
    {
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.spatialBlend = 0f;
        _sfxSource.volume = soundVolume;
    }

    private void Update()
    {
        bool detected = DetectObject();

        if (detected != isPressed)
        {
            isPressed = detected;
            UpdateVisuals();
            PlaySound();
        }
    }
    private bool DetectObject()
    {
        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out RaycastHit hit, detectionRange))
            return false;

        if (selectObject)
        {
            return hit.transform.gameObject.Equals(_selectObject);
        }
        
        return hit.transform.CompareTag(PLAYER_TAG) || hit.transform.CompareTag(PHYSICS_TAG);
    }

    private void UpdateVisuals()
    {
        pressedVisual.SetActive(isPressed);
        normalVisual.SetActive(!isPressed);
    }

    private void PlaySound()
    {
        if (pressSound != null)
            _sfxSource.PlayOneShot(pressSound);
    }

    public bool IsButtonPressed() => isPressed;
}