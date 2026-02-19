using System;
using TMPro;
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
    
    private bool ControlTabOpen = false;
    private Vector3 guiObjectEndPosition;
    private Vector3 menuEndPosition;
    private Vector3 controlTabEndPos;
    
    
    
    protected GameObject NearestObjectOfTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        float nearestDistance = 100000;
        GameObject nearest = null;
        float distance;

        for (int i = 0; i < objects.Length; i++)
        {
            distance = Vector3.Distance(transform.position, objects[i].transform.position);

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
        guiObjectEndPosition = GUIObject.transform.position  + Vector3.down * 150f;
        controlTabEndPos = controlTab.transform.position;
        menuEndPosition = menu.transform.position;
    }

    private void Update()
    {
        if (transform.position.y < -3)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                if (ControlTabOpen)
                {
                    controlTabEndPos = controlTab.transform.position + Vector3.right * 1000f;
                    menuEndPosition = menu.transform.position + Vector3.right * 1000f;
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
        
        GUIObject.transform.position = Vector3.Lerp(GUIObject.transform.position, guiObjectEndPosition,  Time.unscaledDeltaTime * 2f);
        controlTab.transform.position = Vector3.Lerp(controlTab.transform.position, controlTabEndPos, Time.unscaledDeltaTime * 2f);
        menu.transform.position = Vector3.Lerp(menu.transform.position, menuEndPosition, Time.unscaledDeltaTime * 2f);
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
        controlTabEndPos = controlTab.transform.position + Vector3.left * 1000f;
        menuEndPosition = menu.transform.position + Vector3.left * 1000f;
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