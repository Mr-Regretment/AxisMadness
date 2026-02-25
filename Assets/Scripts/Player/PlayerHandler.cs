using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerHandler : Entity
{
    public static bool GameIsPaused = false;
    
    [SerializeField] protected float moveSpeed;
    [SerializeField] public int tokenCount;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject controlTab;
    [SerializeField] private GameObject GUIObject;
    
    private RectTransform menuRect;
    private RectTransform controlTabRect;
    private RectTransform guiObjectRect;
    
    private bool ControlTabOpen = false;
    private Vector2 guiObjectEndPosition;
    private Vector2 menuEndPosition;
    private Vector2 controlTabEndPos;

    protected GameObject NearestObjectOfTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        float nearestDistance = 100000;
        GameObject nearest = null;

        for (int i = 0; i < objects.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, objects[i].transform.position);
            if (distance < nearestDistance)
            {
                nearest = objects[i];
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    private void Start()
    {
        if(menu != null)
            menuRect = menu.GetComponent<RectTransform>();
        if(controlTab != null)
            controlTabRect = controlTab.GetComponent<RectTransform>();
        if(GUIObject != null)
            guiObjectRect = GUIObject.GetComponent<RectTransform>();

        if(guiObjectRect != null)
            guiObjectEndPosition = guiObjectRect.anchoredPosition + (Vector2.down * 250f) + (Vector2.right * 100f);
        if(controlTabRect != null)
            controlTabEndPos = controlTabRect.anchoredPosition;
        if(menuRect != null)
            menuEndPosition = menuRect.anchoredPosition;
    }

    private void Update()
    {
        if (transform.position.y < -3 && !IsGrounded())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !player.GetComponent<PlayerCamera>().StandingOverRotatePad())
        {
            if (GameIsPaused)
            {
                if (ControlTabOpen)
                {
                    controlTabEndPos = controlTabRect.anchoredPosition + Vector2.right * 1300f;
                    menuEndPosition = menuRect.anchoredPosition + Vector2.right * 1300f;
                    ControlTabOpen = false;
                }
                else
                {
                    Resume();
                }
            }
            else
            {
                Pause();
            }
        }

        guiObjectRect.anchoredPosition = Vector2.Lerp(guiObjectRect.anchoredPosition, guiObjectEndPosition, Time.unscaledDeltaTime * 2f);
        controlTabRect.anchoredPosition = Vector2.Lerp(controlTabRect.anchoredPosition, controlTabEndPos, Time.unscaledDeltaTime * 2f);
        menuRect.anchoredPosition = Vector2.Lerp(menuRect.anchoredPosition, menuEndPosition, Time.unscaledDeltaTime * 2f);
    }

    public void Resume()
    {
        menu.SetActive(false);
        GameIsPaused = !GameIsPaused;
        Time.timeScale = 1;
    }

    public void Pause()
    {
        menu.SetActive(true);
        GameIsPaused = !GameIsPaused;
        Time.timeScale = 0;
    }

    public void Controls()
    {
        controlTabEndPos = controlTabRect.anchoredPosition + Vector2.left * 1300f;
        menuEndPosition = menuRect.anchoredPosition + Vector2.left * 1300f;
        ControlTabOpen = true;
    }

    public void StopGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    protected GameObject NearestObjectOfTagWithComponent(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (var obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }

        return nearest;
    }
}