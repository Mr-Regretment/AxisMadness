using System;
using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    public bool hasTouchedPlayer = false;
    [SerializeField] public String[] text;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasTouchedPlayer = true;
            Invoke(nameof(Destroy), 2.5f);
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
    
}