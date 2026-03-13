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

        [Header("Dialogue")]
        public DialogueHandler dialogueHandler;
        public string[] text;
        public float speed;
        public float waitTime;
        public bool CanSkip;

        [Header("Camera Reposition")]
        public CameraHandler cameraHandler;
        public Vector3 cameraTargetPosition;
        public float cameraDuration;

        [Header("Move Settings")]
        public float moveDistance = 20f;
        public bool moveUpward = true;
        public float moveSpeed = 2f;
        public bool despawn = true;
        public bool stayOnRelease = false;

        [HideInInspector] public Vector3 restPosition;
        [HideInInspector] public bool restPositionSet = false;
        [HideInInspector] public bool wasPressed;
        [HideInInspector] public bool hasReachedTarget;
        [HideInInspector] public bool dialoguePlayed;
    }

    [SerializeField] private List<ButtonEffect> effects = new List<ButtonEffect>();

    private void Update()
    {
        foreach (ButtonEffect effect in effects)
        {
            if (effect.button == null)
                continue;

            bool isPressed = effect.button.IsButtonPressed();

            if (isPressed && !effect.wasPressed)
                OnButtonPressed(effect);

            if (!isPressed && effect.wasPressed)
                OnButtonReleased(effect);

            if (effect.objectToToggle != null)
                HandleToggle(effect, isPressed);

            if (effect.objectToMove != null)
                MoveObject(effect, isPressed);

            effect.wasPressed = isPressed;
        }
    }

    private void OnButtonPressed(ButtonEffect effect)
    {
        if (effect.dialogueHandler != null && effect.text != null &&
            effect.text.Length > 0 && !effect.dialoguePlayed)
        {
            effect.dialogueHandler.ShowDialogueWithSpeeds(effect.text, effect.speed, effect.waitTime, effect.CanSkip);
            effect.dialoguePlayed = true;
        }

        if (effect.cameraHandler != null && effect.cameraTargetPosition != Vector3.zero)
            effect.cameraHandler.CameraMove(effect.cameraTargetPosition,true,effect.cameraDuration);
    }

    private void OnButtonReleased(ButtonEffect effect)
    {
        if (!effect.stayOnRelease)
            effect.hasReachedTarget = false;
    }

    private void MoveObject(ButtonEffect effect, bool isPressed)
    {
        if (!effect.restPositionSet)
        {
            effect.restPosition = effect.objectToMove.transform.localPosition;
            effect.restPositionSet = true;
        }

        Vector3 direction = effect.moveUpward ? Vector3.up : Vector3.down;
        Vector3 movedPosition = effect.restPosition + direction * effect.moveDistance;

        Vector3 target = (isPressed || (effect.stayOnRelease && effect.hasReachedTarget))
            ? movedPosition
            : effect.restPosition;

        effect.objectToMove.transform.localPosition = Vector3.Lerp(
            effect.objectToMove.transform.localPosition,
            target,
            Time.deltaTime * effect.moveSpeed
        );

        if (isPressed && Vector3.Distance(effect.objectToMove.transform.localPosition, movedPosition) < 2f)
        {
            effect.hasReachedTarget = true;
            if (effect.despawn)
                effect.objectToMove.SetActive(false);
        }
        else if (!isPressed && !effect.stayOnRelease && effect.despawn)
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