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
    private bool hasFadedOut = false;

    public bool IsStopped => isStopped;
    public bool IsStoppedAndFaded => isStopped && hasFadedOut;

    void Start()
    {
        videoPlayer.enabled = true;
        videoPlayer.Play();
    }

    void Update()
    {
        if (videoPlayer == null)
        {
            isStopped = true;
            return;
        }

        if (!hasVideoStarted && videoPlayer.isPlaying)
            hasVideoStarted = true;

        if (hasVideoStarted && !videoPlayer.isPlaying && !isStopped)
        {
            isStopped = true;
            StartCoroutine(FadeOutVideo(1f));
        }
    }

    IEnumerator FadeOutVideo(float duration)
    {
        yield return new WaitForSeconds(0.5f);

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

        
        hasFadedOut = true;
    }
}