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
        [Tooltip("Lines to display in sequence.")]
        public List<string> lines = new List<string>();

        [Header("Trigger Condition")]
        [Tooltip("Optional: fires when this method returns true. Leave empty to fire immediately.")]
        public MonoBehaviour triggerScript;
        public string triggerMethodName;

        public float speed = 0.05f;
        public float waitTime = 2.5f;
        public bool canSkip = true;

        [HideInInspector] public bool hasPlayed = false;

        public bool IsTriggerMet()
        {
            if (triggerScript == null || string.IsNullOrEmpty(triggerMethodName))
                return true;

            MethodInfo method = triggerScript.GetType().GetMethod(triggerMethodName);
            if (method != null && method.ReturnType == typeof(bool))
                return (bool)method.Invoke(triggerScript, null);

            Debug.LogWarning($"DialogueLine: Trigger method '{triggerMethodName}' not found or doesn't return bool on {triggerScript.GetType().Name}");
            return false;
        }
    }

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject skipTextPanel;
    [SerializeField] private GameObject cameraHandler;
    [SerializeField] private PlayerCamera playerCamera;

    [Header("Dialogue")]
    [SerializeField] private List<DialogueLine> dialogueLines = new List<DialogueLine>();

    private TextMeshProUGUI _dialogueText;

    private bool _isReady;
    private bool _isTypingFinished;
    private bool _skipLine;
    private bool _isDialogueActive;

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

        if (!_isDialogueActive)
        {
            foreach (DialogueLine entry in dialogueLines)
            {
                if (!entry.hasPlayed && entry.IsTriggerMet())
                {
                    entry.hasPlayed = true;
                    StartCoroutine(ShowDialogueEntry(entry));
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
            _skipLine = true;

        dialoguePanel.transform.position = Vector3.Lerp(
            dialoguePanel.transform.position, targetPosition, Time.deltaTime * 4f);
    }

    private IEnumerator ShowDialogueEntry(DialogueLine entry)
    {
        string[] lines = entry.lines.ToArray();
        float[] speeds = new float[lines.Length];
        float[] waitTimes = new float[lines.Length];
        bool[] canSkips = new bool[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            speeds[i] = entry.speed;
            waitTimes[i] = entry.waitTime;
            canSkips[i] = entry.canSkip;
        }

        yield return StartCoroutine(ShowDialogueWithSpeeds(lines, speeds, waitTimes, canSkips));
    }

    public IEnumerator ShowDialogue(string[] lines, float speed)
    {
        float[] speeds = new float[lines.Length];
        float[] waitTimes = new float[lines.Length];
        bool[] canSkips = new bool[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            speeds[i] = speed;
            waitTimes[i] = 2.5f;
            canSkips[i] = true;
        }

        yield return StartCoroutine(ShowDialogueWithSpeeds(lines, speeds, waitTimes, canSkips));
    }

    public IEnumerator ShowDialogue(string text, float speed)
    {
        yield return StartCoroutine(ShowDialogue(new string[] { text }, speed));
    }

    private IEnumerator ShowDialogueWithSpeeds(string[] lines, float[] speeds, float[] waitTimes, bool[] canSkips)
    {
        if (_isDialogueActive) yield break;
        _isDialogueActive = true;

        targetPosition = hiddenPosition + Vector3.down * 275;
        player.GetComponent<PlayerMovement>().ShouldMove = false;

        yield return new WaitUntil(() =>
            Vector3.Distance(dialoguePanel.transform.position, targetPosition) < 1f);

        for (int i = 0; i < lines.Length; i++)
        {
            skipTextPanel.SetActive(canSkips[i]);

            _isTypingFinished = false;
            StartCoroutine(AnimateText(lines[i], speeds[i]));
            yield return new WaitUntil(() => _isTypingFinished);

            float timer = 0f;
            while (timer < waitTimes[i] && !_skipLine)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            _skipLine = false;
        }

        skipTextPanel.SetActive(false);
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