using UnityEngine;

public class PlayerMovement : PlayerHandler
{
    public bool ShouldMove;
    [SerializeField] private bool displayShouldMove;
    [SerializeField] private GameObject GeometryGameObject;
    protected void FixedUpdate()
    {
        displayShouldMove = ShouldMove;
        HandleMovement();
    }

    protected void Update()
    {
        HandleJumping();
        HandleChildRotation();
    }

    private void HandleMovement()
    {
        moveSpeed = 10;
        float vertical = Input.GetAxis("Horizontal");
    
        float speed = ShouldMove ? vertical * moveSpeed : 0f;
        Vector3 movement = transform.forward * speed;
        rigidbody.linearVelocity = new Vector3(movement.x, rigidbody.linearVelocity.y, movement.z);
    }
    
    private void HandleChildRotation()
    {
        if (GeometryGameObject == null)
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
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void ShouldMoveSet(bool shouldMoveSet)
    {
        bool hasSetBool = false;
        if (!hasSetBool)
        {
            ShouldMove = shouldMoveSet;
            hasSetBool = true;
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
        {
            return 1.5f * Mathf.Abs(rigidbody.linearVelocity.x);
        }
        return 1.5f * Mathf.Abs(rigidbody.linearVelocity.z);
    }

    public bool IsJumping()
    {
        if(IsGrounded())
            return false;
        
        return rigidbody.linearVelocity.y > 0.1f;
    }

    public bool IsMoving()
    {
        float minVelocity = 0.1f;
        return Mathf.Abs(rigidbody.linearVelocity.x) > minVelocity || 
               Mathf.Abs(rigidbody.linearVelocity.z) > minVelocity;
    }

    public bool IsFalling()
    {
        if (IsGrounded())
            return false;
    
        return rigidbody.linearVelocity.y < -0.1f;
    }


    public bool IsIdle()
    {
        return rigidbody.linearVelocity.x == 0f || rigidbody.linearVelocity.z == 0f;
    }
    
    
}

