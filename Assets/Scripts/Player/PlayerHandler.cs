using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerHandler : Entity
{
    public static bool GameIsPaused = false;
    
    
    [SerializeField] protected float moveSpeed;
    [SerializeField] public int tokenCount;
    [SerializeField] private GameObject menu;
    
    [SerializeField] private GameObject GUIObject;
    private Vector3 startPosition;
    private Vector3 endPosition;
    
    
    
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
        startPosition = GUIObject.transform.position;
        endPosition = GUIObject.transform.position  + Vector3.down * 150;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        
        GUIObject.transform.position = Vector3.Lerp(GUIObject.transform.position, endPosition,  Time.deltaTime * 2f);
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