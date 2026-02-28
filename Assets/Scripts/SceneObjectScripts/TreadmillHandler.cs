using UnityEngine;

public class TreadmillHandler : MonoBehaviour
{
    [SerializeField] private float TreadmillSpeed;
    private float backAndForthDir = 1f;
    private Collider _treadmillCollider;
    private float _halfLength;

    private void Start()
    {
        _treadmillCollider = GetComponent<Collider>();
        if (_treadmillCollider == null)
            _treadmillCollider = GetComponentInChildren<Collider>();
        _halfLength = Mathf.Abs(transform.InverseTransformVector(_treadmillCollider.bounds.extents).z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 treadmillVelocity = transform.forward * (TreadmillSpeed * backAndForthDir);
        rb.linearVelocity = new Vector3(treadmillVelocity.x, rb.linearVelocity.y, treadmillVelocity.z);

        float localPlayerPos = transform.InverseTransformPoint(other.transform.position).z;

        float insetLength = _halfLength * 0.85f;
        if (localPlayerPos >= insetLength && backAndForthDir > 0f)
            backAndForthDir = -1f;
        else if (localPlayerPos <= -insetLength && backAndForthDir < 0f)
            backAndForthDir = 1f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }
}