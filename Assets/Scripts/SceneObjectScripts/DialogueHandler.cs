using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        [Tooltip("Static lines to display in sequence.")]
        public List<string> lines = new List<string>();

        [Tooltip("Optional: A MonoBehaviour script with a method that returns a string.")]
        public MonoBehaviour sourceScript;

        [Tooltip("Optional: The name of the method on the source script to call.")]
        public string methodName;

        public float speed = 0.05f;

        public string[] GetLines()
        {
            if (sourceScript != null && !string.IsNullOrEmpty(methodName))
            {
                MethodInfo method = sourceScript.GetType().GetMethod(methodName);
                if (method != null && method.ReturnType == typeof(string))
                    return new string[] { (string)method.Invoke(sourceScript, null) };
                else
                    Debug.LogWarning($"DialogueLine: Method '{methodName}' not found or doesn't return string on {sourceScript.GetType().Name}");
            }
            return lines.ToArray();
        }
    }
    
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject cameraHandler;
    [SerializeField] private ObjectCollision objectCollision;
    [SerializeField] private PlayerCamera playerCamera;
    
    [Header("Dialogue")]
    [SerializeField] private List<DialogueLine> dialogueLines = new List<DialogueLine>();

    private TextMeshProUGUI _dialogueText;

    private bool _isReady;
    private bool _isTypingFinished;
    private bool _dialogueQueued;
    private bool _skipLine;
    private bool _isDialogueActive;
    private bool _hasShownPadDialogue;

    private Vector3 targetPosition;
    private Vector3 hiddenPosition;

    public void Init()
    {
        _dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>(true);

        if (_dialogueText == null)
        {
            Debug.LogError("DialogueHandler: Could not find TextMeshProUGUI in children of " + dialoguePanel.name);
            return;
        }

        hiddenPosition = dialoguePanel.transform.position;
        targetPosition = hiddenPosition + Vector3.up * 175;

        if (cameraHandler == null)
            cameraHandler = FindFirstObjectByType<CameraHandler>().gameObject;

        if (player != null)
        {
            playerCamera = player.GetComponent<PlayerCamera>();
            cameraHandler.GetComponent<CameraHandler>().OverrideShouldMove = true;
            _isReady = true;
        }
    }

    void Start()
    {
        if (dialoguePanel != null && player != null)
            Init();
    }

    void Update()
    {
        if (!_isReady)
            return;

        if (!_dialogueQueued && dialogueLines.Count > 0)
        {
            _dialogueQueued = true;
            StartCoroutine(ShowDialogueLines(dialogueLines));
        }

        if (playerCamera != null && playerCamera.StandingOverRotatePad() && !_hasShownPadDialogue)
        {
            _hasShownPadDialogue = true;
            StartCoroutine(ShowDialogue(new string[]
            {
                "Oh, this is a Rotate Pad and Axis Token!",
                "Rotate Pads can be used to rotate the world around you at the cost of an Axis Token.",
                "Controls for Axis Break: ",
                "Axis Break(Hold Shift) and press Q to rotate it right or E to rotate it left.",
                "Go ahead, try it!"
            }, 0.045f));
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
            _skipLine = true;

        dialoguePanel.transform.position = Vector3.Lerp(
            dialoguePanel.transform.position, targetPosition, Time.deltaTime * 4f);
    }

    // Run a List of DialogueLines (respects per-line speed and dynamic text)
    public IEnumerator ShowDialogueLines(List<DialogueLine> dialogueLines)
    {
        List<string> resolvedLines = new List<string>();
        List<float> speeds = new List<float>();

        foreach (var entry in dialogueLines)
        {
            foreach (var line in entry.GetLines())
            {
                resolvedLines.Add(line);
                speeds.Add(entry.speed);
            }
        }

        yield return StartCoroutine(ShowDialogueWithSpeeds(resolvedLines.ToArray(), speeds.ToArray()));
    }

    // Convenience overload: uniform speed, string array
    public IEnumerator ShowDialogue(string[] lines, float speed)
    {
        float[] speeds = new float[lines.Length];
        for (int i = 0; i < speeds.Length; i++) speeds[i] = speed;
        yield return StartCoroutine(ShowDialogueWithSpeeds(lines, speeds));
    }

    // Convenience overload: single string
    public IEnumerator ShowDialogue(string text, float speed)
    {
        yield return StartCoroutine(ShowDialogue(new string[] { text }, speed));
    }

    private IEnumerator ShowDialogueWithSpeeds(string[] lines, float[] speeds)
    {
        if (_isDialogueActive) yield break;
        _isDialogueActive = true;

        targetPosition = hiddenPosition + Vector3.down * 275;
        player.GetComponent<PlayerMovement>().ShouldMove = false;

        yield return new WaitUntil(() =>
            Vector3.Distance(dialoguePanel.transform.position, targetPosition) < 1f);

        for (int i = 0; i < lines.Length; i++)
        {
            _isTypingFinished = false;
            StartCoroutine(AnimateText(lines[i], speeds[i]));
            yield return new WaitUntil(() => _isTypingFinished);

            float timer = 0f;
            while (timer < 2.5f && !_skipLine)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            _skipLine = false;
        }

        targetPosition = hiddenPosition + Vector3.up * 175;
        cameraHandler.GetComponent<CameraHandler>().OverrideShouldMove = false;
        player.GetComponent<PlayerMovement>().ShouldMove = true;

        _isDialogueActive = false;

        yield return new WaitUntil(() =>
            Vector3.Distance(dialoguePanel.transform.position, targetPosition) < 1f);

        _dialogueText.text = "";
    }

    private IEnumerator AnimateText(string message, float speed = 0.05f)
    {
        _isTypingFinished = false;
        _skipLine = false;
        _dialogueText.text = "";

        foreach (char letter in message)
        {
            if (_skipLine)
            {
                _dialogueText.text = message;
                break;
            }
            _dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(speed);
        }

        _skipLine = false;
        _isTypingFinished = true;
    }
}