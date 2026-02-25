using System;
using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    public bool hasTouchedPlayer = false;
    [SerializeField] public String[] text;
    [SerializeField] public float speed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasTouchedPlayer = true;
            CameraHandler cameraHandler = FindFirstObjectByType<CameraHandler>();
            cameraHandler.RelocateCameraToPlayer();
            Invoke(nameof(Destroy), 2.5f);
        }
    }

    public bool HasBeenActivated()
    {
        return hasTouchedPlayer;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
    
}