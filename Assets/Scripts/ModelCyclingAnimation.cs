using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

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
    [SerializeField] private MonoBehaviour animationController;
    [SerializeField] private string animationControllerMethodName = "";

    public string CurrentAnimationName =>
        animations.Count > 0 ? animations[_currentAnimationIndex].animationName : string.Empty;

    public int CurrentFrameIndex => _currentModelIndex;

    public System.Action<string, int> OnAnimationFrameChanged;

    private int _currentAnimationIndex = 0;
    private int _currentModelIndex = 0;
    private float _timeSinceLastFrame = 0f;
    private MethodInfo _cachedMethod;

    private void Start()
    {
        if (animationController != null && !string.IsNullOrEmpty(animationControllerMethodName))
            _cachedMethod = animationController.GetType().GetMethod(animationControllerMethodName);

        if (animations.Count > 0)
            UpdateModelDisplay();
    }

    private void Update()
    {
        if (animations.Count == 0)
            return;

        if (_cachedMethod != null)
        {
            object result = _cachedMethod.Invoke(animationController, null);
            if (result is string animationName)
                SwitchAnimationByName(animationName);
        }

        ModelAnimation current = animations[_currentAnimationIndex];
        if (!current.isActive)
            return;

        _timeSinceLastFrame += Time.deltaTime;
        if (_timeSinceLastFrame >= 1f / current.speed)
        {
            _timeSinceLastFrame = 0f;
            _currentModelIndex = (_currentModelIndex + 1) % current.models.Length;
            UpdateModelDisplay();
            OnAnimationFrameChanged?.Invoke(current.animationName, _currentModelIndex);
        }
    }

    public void SwitchAnimationByName(string animationName)
    {
        for (int i = 0; i < animations.Count; i++)
        {
            if (animations[i].animationName == animationName && animations[i].isActive)
            {
                if (i == _currentAnimationIndex) return;
                _currentAnimationIndex = i;
                _currentModelIndex = 0;
                _timeSinceLastFrame = 0f;
                UpdateModelDisplay();
                return;
            }
        }
    }

    private void UpdateModelDisplay()
    {
        foreach (ModelAnimation animation in animations)
            foreach (GameObject model in animation.models)
                if (model != null) model.SetActive(false);

        GameObject target = animations[_currentAnimationIndex].models[_currentModelIndex];
        if (target != null) target.SetActive(true);
    }
}