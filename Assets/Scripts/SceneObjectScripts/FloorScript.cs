using UnityEngine;

public class ShaderPropertyOverride : MonoBehaviour
{
    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetFloat("_ObjectWorldBottom", renderer.bounds.min.y);
        block.SetFloat("_ObjectWorldTop", renderer.bounds.max.y);
        renderer.SetPropertyBlock(block);
    }
}