using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoHandler : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoImage;
    private bool hasVideoStarted = false;
    private bool isStopped = false;

    public bool IsStopped => isStopped; // Public property for MenuInitializer

    void Start()
    {
        videoPlayer.Play();
    }

    void Update()
    {
        // Once video is playing, mark it as started
        if (!hasVideoStarted && videoPlayer.isPlaying)
        {
            hasVideoStarted = true;
        }

        // Only fade if: video has started AND now it's stopped AND not already fading
        if (hasVideoStarted && !videoPlayer.isPlaying && !isStopped)
        {
            isStopped = true;
            StartCoroutine(FadeOutVideo(1f));
        }
    }

    IEnumerator FadeOutVideo(float duration)
    {
        yield return new WaitForSeconds(0.5f); // Wait before fading
    
        float elapsed = 0f;
        Color color = videoImage.color;
        float startAlpha = color.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            videoImage.color = color;
            yield return null;
        }

        color.a = 0f;
        videoImage.color = color;
    }
}