using System;
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

        [Header("Text Dropdown")] 
        public String[] text;
        public DialogueHandler dialogueHandler;
        
        [Header("CameraReposition For a Duration")]
        public Vector3 cameraTargetPosition;
        public float duration;
        
        [Header("Settings")]
        public float moveDistance = 20f;
        public bool moveUpward = true;
        public float moveSpeed = 2f;
        
        [HideInInspector] public Vector3 restPosition;
        [HideInInspector] public bool wasPressed;
    }
    
    [SerializeField] private List<ButtonEffect> effects = new List<ButtonEffect>();
    [SerializeField] private CameraHandler cameraHandler;

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

            if (effect.objectToToggle != null)
                HandleToggle(effect, isPressed);

            if (effect.objectToMove != null)
                MoveObject(effect, isPressed);

            if (effect.dialogueHandler != null && isPressed)
                ShowDialogueText(effect, effect.text);

            if (effect.cameraTargetPosition != Vector3.zero && !effect.wasPressed && isPressed)
            {
                effect.wasPressed = true;
                StartCoroutine(cameraHandler.CameraMove(effect.cameraTargetPosition, effect.duration, 1));
            }
        }
    }

    private bool hasPressed = false;

    private void ShowDialogueText(ButtonEffect effect, String[] inputText)
    {
        if (!hasPressed)
        {
            StartCoroutine(effect.dialogueHandler.ShowDialogue(inputText, 0.03f));
            hasPressed = true;
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
                effect.objectToMove.SetActive(false);
        }
        else
        {
            effect.objectToMove.SetActive(true);
        }
    }

    private void HandleToggle(ButtonEffect effect, bool isPressed)
    {
        if (isPressed)
            effect.objectToToggle.SetActive(true);
    }
}