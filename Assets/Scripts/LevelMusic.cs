using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip loopClip;

    private bool _startedMusic = false;
    private VideoHandler _videoHandler;

    private void Start()
    {
        _videoHandler = FindFirstObjectByType<VideoHandler>();
    }

    private void Update()
    {
        if (!_startedMusic && _videoHandler.IsStoppedAndFaded)
        {
            _startedMusic = true;
            Invoke(nameof(StartMusic), 0.5f);
        }
    }

    private void StartMusic()
    {
        BGM.Instance.PlayIntroThenLoop(introClip, loopClip);
    }
}