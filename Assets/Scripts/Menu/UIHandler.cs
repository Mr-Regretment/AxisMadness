using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;
    
    [SerializeField] private Quaternion targetRotation;
    public bool HasAcceptedStartGame;
    public bool IsTransitioning { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HasAcceptedStartGame = false;

        if (scene.buildIndex == 0)
        {
            if (FindFirstObjectByType<MenuInitializer>() == null)
            {
                GameObject prefab = Resources.Load<GameObject>("MenuInitializer");
                if (prefab != null)
                    Instantiate(prefab);
            }
        }
    }

    public void StopGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CameraTransitionToScene(Camera cam, Vector3 targetPosition, Quaternion targetRot, int sceneIndex, float posSpeed = 5f, float rotSpeed = 6f)
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                return;
            }
        }
        StartCoroutine(TransitionCameraCoroutine(cam, targetPosition, targetRot, sceneIndex, posSpeed, rotSpeed));
    }

    private IEnumerator TransitionCameraCoroutine(Camera cam, Vector3 targetPosition, Quaternion targetRot, int sceneIndex, float posSpeed, float rotSpeed)
    {
        IsTransitioning = true;
    
        while (Vector3.Distance(cam.transform.position, targetPosition) > 0.1f || Quaternion.Angle(cam.transform.rotation, targetRot) > 0.1f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, Time.deltaTime * posSpeed);
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRot, Time.deltaTime * rotSpeed);
            yield return null;
        }
    
        IsTransitioning = false;
        SceneManager.LoadScene(sceneIndex);
    }
}