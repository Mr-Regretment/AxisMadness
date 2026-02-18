using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuInitializer : MonoBehaviour
{
    private Vector3 _startPosition;
    [SerializeField] private GameObject TitleScreen;
    [SerializeField] private VideoHandler _videoHandler;
    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject QuitButton;
    [SerializeField] private GameObject SettingsButton;
    [SerializeField] private UIHandler uiHandler;
    private bool animationStarted = false;
    private bool menuMovingBack = false;

    void Start()
    {
        _startPosition = TitleScreen.transform.position;
    }
    
    void Update()
    {
        if (_videoHandler == null)
            return;
        
        if (_videoHandler.IsStopped && !animationStarted)
        {
            animationStarted = true;
            StartCoroutine(TitleScreenAnimation());
        }

        if (uiHandler != null && uiHandler.HasAcceptedStartGame && !menuMovingBack)
        {
            menuMovingBack = true;
            StartCoroutine(MenuPanelMoveBack());
        }
    }

    IEnumerator TitleScreenAnimation()
    {
        yield return StartCoroutine(TitleMoveTo(TitleScreen.transform.position + Vector3.down * 30, 3f));
        
        StartCoroutine(TitleMoveTo(TitleScreen.transform.position + Vector3.right * 10, 3f));
        
        
        
        StartCoroutine(ButtonMoveTo(StartButton, StartButton.transform.position + Vector3.down * 485f, 3f));
        
        StartCoroutine(ButtonMoveTo(QuitButton, QuitButton.transform.position + Vector3.left * 1200f, 3f));
        
        StartCoroutine(ButtonMoveTo(SettingsButton, SettingsButton.transform.position + Vector3.right * 460f, 3f));
        
        _videoHandler.gameObject.SetActive(false);
    }

    IEnumerator MenuPanelMoveBack()
    {
        StartCoroutine(ButtonMoveTo(StartButton, StartButton.transform.position + Vector3.left * 1000f, 3f));
        StartCoroutine(ButtonMoveTo(QuitButton, QuitButton.transform.position + Vector3.left * 1000f, 3f));
        yield return StartCoroutine(ButtonMoveTo(SettingsButton, SettingsButton.transform.position + Vector3.left * 1000f, 3f));
    }

    IEnumerator TitleMoveTo(Vector3 targetPosition, float speed)
    {
        while (TitleScreen != null && Vector3.Distance(TitleScreen.transform.position, targetPosition) > 0.01f)
        {
            TitleScreen.transform.position = Vector3.Lerp(TitleScreen.transform.position, targetPosition, Time.deltaTime * speed);
            yield return null;
        }

        if (TitleScreen != null)
            TitleScreen.transform.position = targetPosition;
    }
    
    IEnumerator ButtonMoveTo(GameObject button, Vector3 targetPosition, float speed)
    {
        if (button == null) yield break;
    
        while (button != null && Vector3.Distance(button.transform.position, targetPosition) > 0.01f)
        {
            button.transform.position = Vector3.Lerp(button.transform.position, targetPosition, Time.deltaTime * speed);
            yield return null;
        }

        if (button != null)
            button.transform.position = targetPosition;
    }
}