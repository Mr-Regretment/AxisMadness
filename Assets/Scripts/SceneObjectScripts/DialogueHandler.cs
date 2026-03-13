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
        public List<string> lines = new List<string>();

        public MonoBehaviour triggerScript;
        public string triggerMethodName;

        public float speed = 0.05f;
        public float waitTime = 2.5f;
        public bool canSkip = true;

        [Header("Persistence")]
        public string dialogueID;
        public bool persistAcrossReloads = false;

        [HideInInspector] public bool hasPlayed = false;

        public bool IsTriggerMet()
        {
            if (triggerScript == null || string.IsNullOrEmpty(triggerMethodName))
                return true;

            MethodInfo method = triggerScript.GetType().GetMethod(triggerMethodName);

            if (method != null && method.ReturnType == typeof(bool))
                return (bool)method.Invoke(triggerScript, null);

            return false;
        }
    }

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject skipTextPanel;

    [Header("Dialogue")]
    [SerializeField] private List<DialogueLine> dialogueLines = new List<DialogueLine>();

    public static HashSet<string> playedDialogues = new HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ClearOnPlay()
    {
        playedDialogues.Clear();
    }

    private TextMeshProUGUI dialogueText;

    private bool isReady;
    private bool isTypingFinished;
    private bool skipLine;
    private bool isDialogueActive;

    private Vector3 targetPosition;
    private Vector3 hiddenPosition;

    private PlayerMovement playerMovement;

    void Start()
    {
        if (dialoguePanel == null || player == null) return;

        dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>(true);

        if (dialogueText == null)
            return;

        playerMovement = player.GetComponent<PlayerMovement>();

        hiddenPosition = dialoguePanel.transform.position;
        targetPosition = hiddenPosition;

        isReady = true;
    }

    void Update()
    {
        if (!isReady) return;

        if (!isDialogueActive)
        {
            foreach (DialogueLine entry in dialogueLines)
            {
                if (entry.hasPlayed) continue;
                if (!entry.IsTriggerMet()) continue;

                if (entry.persistAcrossReloads && playedDialogues.Contains(entry.dialogueID))
                {
                    entry.hasPlayed = true;
                    continue;
                }

                entry.hasPlayed = true;

                if (entry.persistAcrossReloads && !string.IsNullOrEmpty(entry.dialogueID))
                    playedDialogues.Add(entry.dialogueID);

                StartCoroutine(ShowDialogueEntry(entry));
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
            skipLine = true;

        dialoguePanel.transform.position = Vector3.Lerp(
            dialoguePanel.transform.position,
            targetPosition,
            Time.deltaTime * 6f
        );
    }

    private IEnumerator ShowDialogueEntry(DialogueLine entry)
    {
        yield return StartCoroutine(
            ShowDialogueWithSpeeds(
                entry.lines.ToArray(),
                entry.speed,
                entry.waitTime,
                entry.canSkip
            )
        );
    }

    public IEnumerator ShowDialogueWithSpeeds(string[] lines, float speed, float waitTime, bool canSkip)
    {
        if (isDialogueActive) yield break;

        isDialogueActive = true;

        targetPosition = hiddenPosition + Vector3.down * 275;

        if (playerMovement != null)
            playerMovement.ShouldMove = false;

        yield return new WaitForSeconds(0.3f);

        foreach (string line in lines)
        {
            skipTextPanel.SetActive(canSkip);

            isTypingFinished = false;
            StartCoroutine(AnimateText(line, speed));

            yield return new WaitUntil(() => isTypingFinished);

            float timer = 0f;
            while (timer < waitTime && !skipLine)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            skipLine = false;
        }

        skipTextPanel.SetActive(false);

        targetPosition = hiddenPosition;

        if (playerMovement != null)
            playerMovement.ShouldMove = true;

        isDialogueActive = false;

        yield return new WaitForSeconds(0.3f);

        dialogueText.text = "";
    }

    public void MarkAllPlayed()
    {
        foreach (DialogueLine entry in dialogueLines)
            entry.hasPlayed = true;
    }

    private IEnumerator AnimateText(string message, float speed)
    {
        isTypingFinished = false;
        skipLine = false;
        dialogueText.text = "";

        foreach (char letter in message)
        {
            if (skipLine)
            {
                dialogueText.text = message;
                break;
            }

            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(speed);
        }

        isTypingFinished = true;
    }
}