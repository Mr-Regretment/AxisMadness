using UnityEngine;
using System.Collections.Generic;

public class ModelCyclingAnimation : MonoBehaviour
{
    [System.Serializable]
    public class ModelAnimation
    {
        public string animationName = "Animation";
        public GameObject[] models = new GameObject[3];
        public float speed = 2f;
        public bool isActive = true;
    }

    [SerializeField] private List<ModelAnimation> animations = new List<ModelAnimation>();
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CameraHandler cameraHandler;
    [SerializeField] private MonoBehaviour animationController;
    [SerializeField] private string animationControllerMethodName = "";
    
    private int currentAnimationIndex = 0;
    private int currentModelIndex = 0;
    private float timeSinceLastFrame = 0f;

    private void Start()
    {
        if (animations.Count > 0)
            UpdateModelDisplay();
    }

    private void Update()
    {
        if (animations.Count == 0)
            return;

        if (animationController != null && !string.IsNullOrEmpty(animationControllerMethodName))
        {
            var method = animationController.GetType().GetMethod(animationControllerMethodName);
            if (method != null)
            {
                object result = method.Invoke(animationController, null);
                if (result is string animationName)
                {
                    SwitchAnimationByName(animationName);
                }
            }
        }
        else if (playerMovement != null)
        {
            if (cameraHandler != null && cameraHandler.IsRotatingAnimation())
            {
            }
            else if (playerMovement.IsJumping() && CanPlayAnimation(0))
            {
                SwitchAnimation(0);
            }
            else if (playerMovement.IsFalling() && CanPlayAnimation(3))
            {
                SwitchAnimation(3);
            }
            else if (playerMovement.IsMoving() && !playerMovement.IsOnTreadMill() && CanPlayAnimation(1))
            {
                SwitchAnimation(1);
            }
            else if (CanPlayAnimation(2))
            {
                SwitchAnimation(2);
            }
        }

        if (!animations[currentAnimationIndex].isActive)
            return;

        float frameDuration = 1f / animations[currentAnimationIndex].speed;
        timeSinceLastFrame += Time.deltaTime;
    
        if (timeSinceLastFrame >= frameDuration)
        {
            timeSinceLastFrame = 0f;
            currentModelIndex = (currentModelIndex + 1) % animations[currentAnimationIndex].models.Length;
            UpdateModelDisplay();
        }
    }

    private bool CanPlayAnimation(int index)
    {
        if (index < 0 || index >= animations.Count)
            return false;
        
        return animations[index].isActive;
    }

    private void SwitchAnimation(int index)
    {
        index = Mathf.Clamp(index, 0, animations.Count - 1);
        
        if (!CanPlayAnimation(index))
            return;
        
        if (index != currentAnimationIndex)
        {
            currentAnimationIndex = index;
            currentModelIndex = 0;
            timeSinceLastFrame = 0f;
            UpdateModelDisplay();
        }
    }

    public void SwitchAnimationByName(string animationName)
    {
        for (int i = 0; i < animations.Count; i++)
        {
            if (animations[i].animationName == animationName)
            {
                SwitchAnimation(i);
                return;
            }
        }
    }

    private void UpdateModelDisplay()
    {
        foreach (ModelAnimation animation in animations)
        {
            foreach (GameObject model in animation.models)
            {
                if (model != null)
                    SetActiveRecursive(model, false);
            }
        }

        if (animations[currentAnimationIndex].models[currentModelIndex] != null)
        {
            SetActiveRecursive(animations[currentAnimationIndex].models[currentModelIndex], true);
        }
    }

    private void SetActiveRecursive(GameObject obj, bool state)
    {
        obj.SetActive(state);
        foreach (Transform child in obj.transform)
            SetActiveRecursive(child.gameObject, state);
    }
}