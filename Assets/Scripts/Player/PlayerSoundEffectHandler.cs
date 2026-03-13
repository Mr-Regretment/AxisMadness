using UnityEngine;

public class PlayerSoundEffectHandler : MonoBehaviour
{
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip walkSound;

    [SerializeField] private float landVelocityThreshold = 3f;

    [Header("Walk Sound Frames")]
    [SerializeField] private int[] includeFrames;
    [SerializeField] private int[] excludeFrames;

    private AudioSource _sfxSource;
    private AudioSource _walkAudioSource;
    private PlayerHandler _playerHandler;
    private Rigidbody _rb;
    private ModelCyclingAnimation _modelCyclingAnimation;

    private bool _wasGrounded = false;
    private float _velocityOnLand = 0f;

    private void Start()
    {
        _playerHandler = GetComponent<PlayerHandler>();
        _rb = GetComponent<Rigidbody>();
        _modelCyclingAnimation = GetComponentInChildren<ModelCyclingAnimation>();

        if (_modelCyclingAnimation != null)
            _modelCyclingAnimation.OnAnimationFrameChanged += OnAnimationFrame;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.spatialBlend = 0f;
        _sfxSource.playOnAwake = false;

        _walkAudioSource = gameObject.AddComponent<AudioSource>();
        _walkAudioSource.clip = walkSound;
        _walkAudioSource.loop = true;
        _walkAudioSource.spatialBlend = 0f;
        _walkAudioSource.playOnAwake = false;

        _sfxSource.Play();
        _sfxSource.Stop();
        _walkAudioSource.Play();
        _walkAudioSource.Stop();
    }

    private void OnDestroy()
    {
        if (_modelCyclingAnimation != null)
            _modelCyclingAnimation.OnAnimationFrameChanged -= OnAnimationFrame;
    }

    private void OnAnimationFrame(string animationName, int frameIndex)
    {
        if (animationName != "Walk" || walkSound == null)
            return;

        if (includeFrames.Length > 0 && System.Array.IndexOf(includeFrames, frameIndex) < 0)
            return;

        if (System.Array.IndexOf(excludeFrames, frameIndex) >= 0)
            return;

        _sfxSource.pitch = Random.Range(0.9f, 1.1f);
        _sfxSource.PlayOneShot(walkSound, Random.Range(0.25f, 0.45f));
    }

    private void FixedUpdate()
    {
        bool isGrounded = _playerHandler.IsGrounded();
        bool justLanded = isGrounded && !_wasGrounded;

        if (!isGrounded && _rb != null)
            _velocityOnLand = Mathf.Abs(_rb.linearVelocity.y);

        _wasGrounded = isGrounded;

        if (justLanded && landSound != null && _velocityOnLand >= landVelocityThreshold)
            _sfxSource.PlayOneShot(landSound, 0.25f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _playerHandler.IsGrounded() && jumpSound != null)
            _sfxSource.PlayOneShot(jumpSound);
    }
}