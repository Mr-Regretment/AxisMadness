using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CameraHandler cameraHandler;
    
    public string GetAnimation()
    {
        if (cameraHandler != null && cameraHandler.IsRotatingAnimation())
            return "Rotating";

        if (playerMovement.IsJumping())
            return "Jump";

        if (playerMovement.IsFalling())
            return "Fall";

        if (playerMovement.IsMoving())
            return "Walk";

        return "Idle";
    }
}
