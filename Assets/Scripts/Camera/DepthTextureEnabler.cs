using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DepthTextureEnabler : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}