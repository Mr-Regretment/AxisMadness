using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class MenuInitializer : MonoBehaviour
{
    private Vector3 _startPosition;
    [SerializeField] private GameObject TitleScreen;
    [SerializeField] private VideoHandler _videoHandler;
    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject QuitButton;
    [SerializeField] private GameObject BackgroundPanel;
    [SerializeField] private UIHandler uiHandler;

    [SerializeField] private float QuitFloat;
    [SerializeField] private float StartFloat;
    [SerializeField] private float StartFloatY;
    private bool animationStarted = false;
    private bool menuMovingBack = false;

    void Start()
    {
        _startPosition = TitleScreen.transform.position;
        StartButton.transform.position = new Vector3(StartFloat,StartButton.transform.position.y,StartButton.transform.position.z);
    }
    
    void Update()
    {
        if (_videoHandler == null)
        {
            return;
        }


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


        StartCoroutine(ButtonMoveTo(StartButton, new Vector3(StartFloat,QuitButton.transform.position.y + 150,StartButton.transform.position.z), 3f));
        
        StartCoroutine(ButtonMoveTo(QuitButton, new Vector3(QuitFloat,QuitButton.transform.position.y,QuitButton.transform.position.z), 3f));
        yield return new WaitForSecondsRealtime(0.05f);
        StartCoroutine(ButtonMoveTo(QuitButton, new Vector3(QuitFloat,QuitButton.transform.position.y  + 25 ,QuitButton.transform.position.z), 3f));
        
        StartCoroutine(ButtonMoveTo(BackgroundPanel, new Vector3(260, BackgroundPanel.transform.position.y,BackgroundPanel.transform.position.z), 3f));
        
        _videoHandler.gameObject.SetActive(false);
    }

    IEnumerator MenuPanelMoveBack()
    {
        StartCoroutine(ButtonMoveTo(StartButton, StartButton.transform.position + Vector3.left * 1000f, 3f));
        StartCoroutine(ButtonMoveTo(QuitButton, QuitButton.transform.position + Vector3.left * 1000f, 3f));
        StartCoroutine(ButtonMoveTo(BackgroundPanel, BackgroundPanel.transform.position + Vector3.left * 1000, 3f));
        yield return null;
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