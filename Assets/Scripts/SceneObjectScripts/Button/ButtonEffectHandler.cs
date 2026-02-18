using System.Collections.Generic;
using UnityEngine;

public class ButtonEffectHandler : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEffect
    {
        [Header("Input")]
        public ButtonHandler button;
        
        [Header("Objects")]
        public GameObject objectToMove;
        public GameObject objectToToggle;
        
        [Header("Settings")]
        public float moveDistance = 20f;
        public bool moveUpward = true;
        public float moveSpeed = 2f;
        
        [HideInInspector] public Vector3 restPosition;
        [HideInInspector] public bool wasPressed;
    }
    
    [SerializeField] private List<ButtonEffect> effects = new List<ButtonEffect>();

    private void Start()
    {
        foreach (var effect in effects)
        {
            if (effect.objectToMove != null)
                effect.restPosition = effect.objectToMove.transform.position;
        }
    }

    private void Update()
    {
        foreach (var effect in effects)
        {
            if (effect.button == null)
                continue;

            bool isPressed = effect.button.IsButtonPressed();
            
            if(effect.objectToToggle != null)
                HandleToggle(effect, isPressed);
            
            if( effect.objectToMove != null)
                MoveObject(effect, isPressed);
        }
    }

    private void MoveObject(ButtonEffect effect, bool isPressed)
    {
        Vector3 direction = effect.moveUpward ? Vector3.up : Vector3.down;
        Vector3 target = isPressed 
            ? effect.restPosition + direction * effect.moveDistance 
            : effect.restPosition;

        effect.objectToMove.transform.position = Vector3.Lerp(
            effect.objectToMove.transform.position,
            target,
            Time.deltaTime * effect.moveSpeed
        );

        if (isPressed)
        {
            float distance = Vector3.Distance(effect.objectToMove.transform.position, target);
            if (distance < 2f)
            {
                effect.objectToMove.SetActive(false);
            }
        }
        else
        {
            effect.objectToMove.SetActive(true);
        }
    }

    private void HandleToggle(ButtonEffect effect, bool isPressed)
    {
        if (isPressed)
        {
            effect.objectToToggle.SetActive(true);
        }
    }
    
}