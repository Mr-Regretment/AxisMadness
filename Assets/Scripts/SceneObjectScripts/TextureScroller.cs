using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] TreadmillHandler treadmillHandler;
    private Renderer _renderer;
    private MaterialPropertyBlock _block;
    private float _time;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if(treadmillHandler.TextureScrolls)
        { 
            _time += Time.deltaTime;
            _renderer.GetPropertyBlock(_block);
            _block.SetFloat("_ScrollTime", _time);

            bool useCustomSpeed = scrollSpeed != -1f;
            _block.SetFloat("_ScrollSpeed", useCustomSpeed ? scrollSpeed : treadmillHandler.TreadmillSpeedGet);

            _renderer.SetPropertyBlock(_block);
        }
    }
    
}