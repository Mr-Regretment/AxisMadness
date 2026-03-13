using UnityEngine;

public class PlayerMovement : PlayerHandler
{
    public bool ShouldMove;
    public bool ShouldJump;

    [SerializeField] private bool displayShouldMove;
    [SerializeField] private GameObject GeometryGameObject;
    [SerializeField] private CameraHandler cameraHandler;

    private bool _canMove = true;
    public bool ShouldMoveHorizontal = true;

    protected void FixedUpdate()
    {
        displayShouldMove = ShouldMove && _canMove;
        HandleMovement();
    }

    protected new void Update()
    {
        HandleJumping();
        HandleChildRotation();
    }

    public void SetMovementEnabled(bool enabled)
    {
        _canMove = enabled;
    }

    private void HandleMovement()
    {
        moveSpeed = 10;
        float vertical = Input.GetAxis("Horizontal");

        if (!_canMove || !ShouldMove || !ShouldMoveHorizontal || Mathf.Abs(vertical) < 0.01f)
        {
            if (!ExternalForceFromTreadmillActive && !rigidbody.isKinematic)
            {
                Vector3 brakeVelocity = rigidbody.linearVelocity;
                brakeVelocity.x = 0f;
                brakeVelocity.z = 0f;
                rigidbody.linearVelocity = brakeVelocity;
            }
            return;
        }

        if (rigidbody.isKinematic) return;

        float speed = vertical * moveSpeed;
        Vector3 movement = transform.forward * speed;

        Vector3 currentVelocity = rigidbody.linearVelocity;
        Vector3 targetVelocity = new Vector3(movement.x, currentVelocity.y, movement.z);

        Vector3 velocityDiff = targetVelocity - currentVelocity;
        velocityDiff.y = 0f;

        rigidbody.AddForce(velocityDiff, ForceMode.VelocityChange);
    }

    private void HandleChildRotation()
    {
        if (GeometryGameObject == null || !_canMove)
            return;

        float inputX = Input.GetAxis("Horizontal");

        if (Mathf.Abs(inputX) > 0.1f)
        {
            float childRotY = inputX < 0f ? 180f : 0f;
            GeometryGameObject.transform.localRotation = Quaternion.Euler(0, childRotY, 0);
        }
    }

    private void HandleJumping()
    {
        if (!_canMove) return;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && ShouldJump)
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public bool AnimatorShouldMove()
    {
        return rigidbody.linearVelocity.x != 0f || rigidbody.linearVelocity.z != 0f;
    }

    public float WalkingAnimationSpeed()
    {
        float rotationY = transform.eulerAngles.y;
        if (Mathf.Abs(rotationY - 90f) < 5f || Mathf.Abs(rotationY - 270f) < 5f)
            return 1.5f * Mathf.Abs(rigidbody.linearVelocity.x);

        return 1.5f * Mathf.Abs(rigidbody.linearVelocity.z);
    }

    public bool IsJumping()
    {
        if (IsGrounded()) return false;
        return rigidbody.linearVelocity.y > 0.75f;
    }

    public bool IsMoving()
    {
        float minVelocity = 0.1f;
        return Mathf.Abs(rigidbody.linearVelocity.x) > minVelocity ||
               Mathf.Abs(rigidbody.linearVelocity.z) > minVelocity;
    }

    public bool IsFalling()
    {
        if (IsGrounded()) return false;
        return rigidbody.linearVelocity.y < -0.75f;
    }

    public bool IsIdle()
    {
        return rigidbody.linearVelocity.x == 0f && rigidbody.linearVelocity.z == 0f;
    }
}