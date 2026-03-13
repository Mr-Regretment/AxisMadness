using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    public bool hasTouchedPlayer = false;

    [SerializeField] private CameraHandler cameraHandler;

    private bool _activated = false;

    private void Awake()
    {
        if (cameraHandler == null)
            cameraHandler = FindFirstObjectByType<CameraHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_activated) return;
        if (!other.CompareTag("Player")) return;

        _activated = true;
        hasTouchedPlayer = true;

        if (cameraHandler != null)
        {
            cameraHandler.FocusOnPlayer();
        }

        Invoke(nameof(DestroySelf), 2.5f);
    }

    public bool HasBeenActivated()
    {
        return _activated;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}