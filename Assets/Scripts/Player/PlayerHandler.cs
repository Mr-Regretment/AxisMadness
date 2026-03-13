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
    [SerializeField] private GameObject deathMenu;
    
    private RectTransform menuRect;
    private RectTransform controlTabRect;
    private RectTransform guiObjectRect;
    private RectTransform deathMenuRect;
    
    private bool ControlTabOpen = false;
    private Vector2 guiObjectEndPosition;
    private Vector2 menuEndPosition;
    private Vector2 controlTabEndPos;
    private Vector2 deathMenuEndPostion;

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
        if(deathMenu != null)
            deathMenuRect = deathMenu.GetComponent<RectTransform>();

        if(guiObjectRect != null)
            guiObjectEndPosition = guiObjectRect.anchoredPosition + (Vector2.down * 250f) + (Vector2.right * 100f);
        if(controlTabRect != null)
            controlTabEndPos = controlTabRect.anchoredPosition;
        if(menuRect != null)
            menuEndPosition = menuRect.anchoredPosition;

        if(deathMenuRect != null)
            deathMenuEndPostion = deathMenuRect.anchoredPosition;
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
#if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = null;
#endif
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    protected void FreezePlayer()
    {
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    protected void Update()
    {
        if (transform.position.y < -3 && !IsGrounded() && !isDead)
        {
            isDead = true;
            deathMenuEndPostion += Vector2.down * 850f;
            Invoke(nameof(FreezePlayer), 4f);
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
        deathMenuRect.anchoredPosition = Vector2.Lerp(deathMenuRect.anchoredPosition, deathMenuEndPostion, Time.unscaledDeltaTime * 5f);
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