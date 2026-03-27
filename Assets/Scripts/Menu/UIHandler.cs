using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Quaternion targetRotation;
    public bool HasAcceptedStartGame;
    public bool IsTransitioning { get; private set; }

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
        Debug.Log($"CameraTransitionToScene called | sceneIndex: {sceneIndex} | cam: {cam}");
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                Debug.Log("No camera found!");
                return;
            }
        }
        StartCoroutine(TransitionCameraCoroutine(cam, targetPosition, targetRot, sceneIndex, posSpeed, rotSpeed));
    }

    public void StartGame()
    {
        HasAcceptedStartGame = true;
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 endPos = cam.transform.position + Vector3.down * 15.5f;
            CameraTransitionToScene(cam, endPos, targetRotation, 1, 5f, 6f);
        }
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