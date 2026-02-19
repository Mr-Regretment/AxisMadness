using System;
using System.Collections;
using UnityEngine;
public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected new Rigidbody rigidbody;
    [SerializeField] protected GameObject player;
    [SerializeField] protected float jumpForce;

    public int getHealth()
    {
        return health;
    }

    public void setHealth(int value)
    {
        health = value;
    }
    
    protected bool IsGrounded()
    {
        if (rigidbody == null)
            return false;

        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
    
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 2.5f))
            return false;
    
        if (hitInfo.transform == null)
            return false;

        return hitInfo.transform.CompareTag("Floor") || hitInfo.transform.CompareTag("RotatePad") || hitInfo.transform.CompareTag("PhysicsObject") || hitInfo.transform.CompareTag("Button");
    }

    protected void Countdown(float secondsWait, System.Action func)
    {
        StartCoroutine(CountdownTimer(secondsWait, func));
    }

    private IEnumerator CountdownTimer(float secondsWait, System.Action func)
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= secondsWait)
            {
                func.Invoke();
                timer = 0f;
            }
            yield return null;
        }
    }
}