using System;
using UnityEngine;

public class TreadmillHandler : MonoBehaviour
{
    [SerializeField] private float TreadmillSpeed;
    private Collider _treadmillCollider;
    private float _halfLength;
    private bool _reachedEnd = false;
    private bool ObjectOnTop;
    public bool TextureScrolls;

    public float TreadmillSpeedGet
    {
        get => TreadmillSpeed;
    }

    private void Start()
    {
        _treadmillCollider = GetComponent<Collider>();
        if (_treadmillCollider == null)
            _treadmillCollider = GetComponentInChildren<Collider>();
        _halfLength = Mathf.Abs(transform.InverseTransformVector(_treadmillCollider.bounds.extents).z);
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        ObjectOnTop = true;
        Entity entity = other.GetComponent<Entity>();

        float localPos = transform.InverseTransformPoint(other.transform.position).z;
        float insetLength = _halfLength * 0.85f;

        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();

            if (localPos <= -insetLength)
            {
                _reachedEnd = true;
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
                if (pm != null) pm.ExternalForceFromTreadmillActive = false;
                return;
            }

            if (!_reachedEnd)
            {
                bool isTilted = Mathf.Abs(transform.forward.y) > 0.1f;
                bool isGrounded = entity != null && entity.IsGrounded();

                if (pm != null) pm.ExternalForceFromTreadmillActive = isGrounded || isTilted;

                if (isGrounded || isTilted)
                {
                    Vector3 treadmillVelocity = -transform.forward * TreadmillSpeed;
                    Vector3 current = rb.linearVelocity;
                    Vector3 target = new Vector3(treadmillVelocity.x, isTilted ? treadmillVelocity.y : current.y, treadmillVelocity.z);

                    if (current.y > 0.1f) target.y = current.y;

                    rb.AddForce(target - current, ForceMode.VelocityChange);
                }
            }

            return;
        }
        
        if (entity != null) entity.ExternalForceFromTreadmillActive = true;

        Vector3 velocity = -transform.forward * TreadmillSpeed;
        Vector3 nonPhyScurrent = rb.linearVelocity;
        Vector3 nonPhyStarget = new Vector3(velocity.x, nonPhyScurrent.y, velocity.z);
        rb.AddForce(nonPhyStarget - nonPhyScurrent, ForceMode.VelocityChange);
    }

    private void OnTriggerExit(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb == null) return;

            _reachedEnd = false;
            ObjectOnTop = false;

            if (other.CompareTag("Player"))
            {
                PlayerMovement pm = other.GetComponent<PlayerMovement>();
                if (pm != null) pm.ExternalForceFromTreadmillActive = false;
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
                return;
            }

            Entity entity = other.GetComponent<Entity>();
            if (entity != null) entity.ExternalForceFromTreadmillActive = false;
        }

        private void Update()
        {
            TextureScrolls = ObjectOnTop && !_reachedEnd;
        }
}