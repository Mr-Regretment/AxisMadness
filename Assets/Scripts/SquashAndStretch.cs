using System.Collections;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject geometryGameObject;

    [Header("Jump")]
    [SerializeField] private Vector3 jumpScale = new Vector3(0.7f, 1.4f, 0.7f);
    [SerializeField] private float jumpScaleSpeed = 10f;

    [Header("Fall")]
    [SerializeField] private Vector3 fallScale = new Vector3(1.2f, 0.8f, 1.2f);

    [Header("Land")]
    [SerializeField] private Vector3 landScale = new Vector3(1.4f, 0.6f, 1.4f);
    [SerializeField] private float landDuration = 0.15f;

    private Vector3 _targetScale = Vector3.one;
    private bool _wasGrounded = true;
    private bool _wasJumping = false;
    private bool _wasFalling = false;
    private bool _isLanding = false;

    private void Update()
    {
        if (geometryGameObject == null || playerMovement == null)
            return;

        bool grounded = playerMovement.IsGrounded();
        bool jumping  = playerMovement.IsJumping();
        bool falling  = playerMovement.IsFalling();

        if (jumping && !_wasJumping)
        {
            _targetScale = jumpScale;
        }
        else if (falling && !_wasFalling)
            _targetScale = fallScale;
        else if (grounded && !_wasGrounded)
            StartCoroutine(LandSquash());
        else if (grounded && !jumping && !falling && !_isLanding)
            _targetScale = Vector3.one;

        _wasGrounded = grounded;
        _wasJumping  = jumping;
        _wasFalling  = falling;

        geometryGameObject.transform.localScale = Vector3.Lerp(
            geometryGameObject.transform.localScale,
            _targetScale,
            Time.deltaTime * jumpScaleSpeed
        );

        float scaleY = geometryGameObject.transform.localScale.y;
        float originalHeight = 1f;
        geometryGameObject.transform.localPosition = new Vector3(
            geometryGameObject.transform.localPosition.x,
            (scaleY - originalHeight) * 0.5f,
            geometryGameObject.transform.localPosition.z
        );
    }

    private IEnumerator LandSquash()
    {
        _isLanding = true;
        _targetScale = landScale;
        yield return new WaitForSecondsRealtime(landDuration);
        _targetScale = Vector3.one;
        _isLanding = false;
    }
}