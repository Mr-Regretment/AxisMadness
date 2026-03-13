using System.Collections;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private static BGM _instance;
    public static BGM Instance => _instance;

    [SerializeField] private float fadeSpeed = 1.5f;

    private AudioSource _sourceA;
    private AudioSource _sourceB;
    private bool _playingOnA = true;

    private Coroutine _fadeCoroutine;

    private AudioSource _loopSource;
    private float _lastPlaybackPosition = 0f;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _sourceA = gameObject.AddComponent<AudioSource>();
        _sourceB = gameObject.AddComponent<AudioSource>();
        _sourceA.loop = true;
        _sourceB.loop = true;
        _sourceA.volume = 0f;
        _sourceB.volume = 0f;
    }

    private float _debugLogTimer = 0f;
    private float _debugLogInterval = 1f;

    private void Update()
    {
        if (_loopSource != null && _loopSource.isPlaying && _loopSource.clip != null)
        {
            float current = _loopSource.time;
            float total = _loopSource.clip.length;
            float percentage = (current / total) * 100f;

            _debugLogTimer += Time.deltaTime;
            if (_debugLogTimer >= _debugLogInterval)
            {
                _debugLogTimer = 0f;
                Debug.Log($"[BGM] Loop progress: {percentage:F1}%");
            }

            if (current < _lastPlaybackPosition)
                Debug.Log("[BGM] Loop point hit — restarting loop");

            _lastPlaybackPosition = current;
        }
    }

    public bool IsPlaying
    {
        get
        {
            AudioSource current = _playingOnA ? _sourceA : _sourceB;
            return current.isPlaying;
        }
    }

    public void PlayTrack(AudioClip clip)
    {
        if (clip == null) return;

        AudioSource current = _playingOnA ? _sourceA : _sourceB;
        AudioSource next    = _playingOnA ? _sourceB : _sourceA;

        if (current.clip == clip && current.isPlaying) return;

        next.clip = clip;
        next.Play();

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(Crossfade(current, next));
        _playingOnA = !_playingOnA;
    }

    public void StopMusic()
    {
        AudioSource current = _playingOnA ? _sourceA : _sourceB;

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeOut(current));
    }

    private Coroutine _introCoroutine;

    public void PlayIntroThenLoop(AudioClip intro, AudioClip loop)
    {
        if (_introCoroutine != null)
            StopCoroutine(_introCoroutine);
        _introCoroutine = StartCoroutine(IntroThenLoopRoutine(intro, loop));
    }

    private IEnumerator IntroThenLoopRoutine(AudioClip intro, AudioClip loop)
    {
        AudioSource introSource = _playingOnA ? _sourceB : _sourceA;
        AudioSource loopSource  = _playingOnA ? _sourceA : _sourceB;

        introSource.loop = false;
        introSource.clip = intro;
        introSource.volume = 1f;

        double startDspTime = AudioSettings.dspTime;
        introSource.Play();

        loopSource.clip = loop;
        loopSource.loop = true;
        loopSource.volume = 1f;
        loopSource.PlayScheduled(startDspTime + intro.length);

        yield return new WaitForSeconds(intro.length);

        introSource.Stop();
        _playingOnA = !_playingOnA;

        _loopSource = loopSource;
        _lastPlaybackPosition = 0f;
        Debug.Log("[BGM] Loop started");
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        if (from.volume <= 0f || !from.isPlaying)
        {
            to.volume = 1f;
            from.Stop();
            yield break;
        }

        float fromStart = from.volume;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            from.volume = Mathf.Lerp(fromStart, 0f, t);
            yield return null;
        }

        from.volume = 0f;
        from.Stop();

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            to.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        to.volume = 1f;
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        float startVolume = source.volume;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            source.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }
}