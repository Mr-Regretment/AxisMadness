using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [Header("Tutorial")]
    
    [SerializeField] GameObject player;
    [SerializeField] GameObject startingText;

    [SerializeField] private GameObject cameraHandler;
    [SerializeField] private ObjectCollision objectCollision;

    [SerializeField] private PlayerCamera playerCamera;
    private TextMeshProUGUI _startingTextTMP;
    
    private bool _startedTutorial;
    private bool _isTypingFinished = false;
    private bool _startedCoroutines;
    
    private Vector3 targetPosition;
    private Vector3 startingPosition;

    void Start()
    {
        _startingTextTMP = startingText.GetComponentInChildren<TextMeshProUGUI>();
        if (cameraHandler == null)
            cameraHandler = FindFirstObjectByType<CameraHandler>().gameObject;

        if (startingText != null)
        {
            startingPosition = startingText.transform.position;
        }
        if (player != null)
        {
            playerCamera = player.GetComponent<PlayerCamera>();
            cameraHandler.GetComponent<CameraHandler>().OverrideShouldMove = true;
            _startedTutorial = true;
        }
    }

    private bool hasActivatedTextDropdown = false;
    private bool _hasShownRotatePadText = false;
    void Update()
    {
        if (!_startedTutorial)
            return;

        if (!_startedCoroutines)
        {
            StartCoroutine(TutorialTextDropDown(new string[]
            {
                "Welcome to the Tutorial!"
            }, 0.03f));
            _startedCoroutines = true;
        }

        if (objectCollision != null && objectCollision.hasTouchedPlayer && !hasActivatedTextDropdown)
        {
            hasActivatedTextDropdown = true;
            objectCollision.hasTouchedPlayer = false;
            StartCoroutine(TutorialTextDropDown(objectCollision.text, 0.03f));
        }

        if (playerCamera.StandingOverRotatePad() && !_hasShownRotatePadText)
        {
            _hasShownRotatePadText = true;
            StartCoroutine(TutorialTextDropDown(new string[]
            {
                "Oh, this is a Rotate Pad! And beside it, an Axis Token (The rotating thing)",
                "If you've got at least one Axis Token, you can stand on this to rotate the world around you!",
                "You've gotta press hold down shift to do it though."
            },0.03f
                ));
        }
        startingText.transform.position = Vector3.Lerp(startingText.transform.position, targetPosition, Time.deltaTime * 4f);
    }

    IEnumerator TutorialTextDropDown(string text, float speed)
    {
        yield return StartCoroutine(TutorialTextDropDown(new string[] { text }, speed));
    }

    public IEnumerator TutorialTextDropDown(string[] texts, float speed)
    {
        targetPosition = startingPosition + Vector3.down * 275;
        player.GetComponent<PlayerMovement>().ShouldMove = false;

        yield return new WaitUntil(() => Vector3.Distance(startingText.transform.position, targetPosition) < 1f);

        foreach (string text in texts)
        {
            _isTypingFinished = false;
            StartCoroutine(TypeText(text, speed));
            yield return new WaitUntil(() => _isTypingFinished);
            yield return new WaitForSeconds(0.5f);
        }

        targetPosition = startingPosition + Vector3.up * 175;
        if (player != null)
        {
            cameraHandler.GetComponent<CameraHandler>().OverrideShouldMove = false;
            player.GetComponent<PlayerMovement>().ShouldMove = true;
        }
        hasActivatedTextDropdown = false;
        yield return new WaitUntil(() => Vector3.Distance(startingText.transform.position, targetPosition) < 1f);
        _startingTextTMP.text = "";
    }
    
    IEnumerator TypeText(string message, float speed = 0.05f)
    {
        _isTypingFinished = false;
        _startingTextTMP.text = "";
        foreach (char letter in message)
        {
            _startingTextTMP.text += letter;
            yield return new WaitForSeconds(speed);
        }
        _isTypingFinished = true;
    }
    
}
