using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject pressedVisual;
    [SerializeField] private GameObject normalVisual;
    [SerializeField] private float detectionRange = 1.2f;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private float soundVolume = 10f;
    
    private bool isPressed;
    private const string PLAYER_TAG = "Player";
    private const string PHYSICS_TAG = "PhysicsObject";

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
        if (!Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, detectionRange))
            return false;

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
            AudioSource.PlayClipAtPoint(pressSound, transform.position, soundVolume);
    }

    public bool IsButtonPressed() => isPressed;
}