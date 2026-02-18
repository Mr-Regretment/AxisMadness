using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    private Material glitchMaterial;
    private Material vignetteMaterial;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CameraHandler cameraHandler;
    [SerializeField] private float glitchIntensity = 0.5f;
    [SerializeField] private float redVignetteIntensity = 0.3f;
    [SerializeField] private float blueVignetteIntensity = 0.3f;
    private bool shouldGlitch = false;

    private void Start()
    {
        if (glitchMaterial == null)
        {
            glitchMaterial = new Material(Shader.Find("Hidden/Kino/Glitch/Analog"));
        }
        
        if (vignetteMaterial == null)
        {
            vignetteMaterial = new Material(Shader.Find("Hidden/Vignette"));
        }
    }

    private void Update()
    {
        if (cameraHandler.IsRotatingAnimation())
            shouldGlitch = true;
        else
            shouldGlitch = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
        
        // Apply glitch
        if (shouldGlitch && glitchMaterial != null)
        {
            glitchMaterial.SetVector("_ScanLineJitter", new Vector2(0.1f * glitchIntensity, 0.5f));
            glitchMaterial.SetVector("_VerticalJump", new Vector2(0.2f * glitchIntensity, Time.time));
            glitchMaterial.SetFloat("_HorizontalShake", 0.5f * glitchIntensity);
            glitchMaterial.SetVector("_ColorDrift", new Vector2(0.05f * glitchIntensity, Time.time));
            Graphics.Blit(source, temp, glitchMaterial);
        }
        else
        {
            Graphics.Blit(source, temp);
        }
        
        // Apply vignette
        if (shouldGlitch && vignetteMaterial != null)
        {
            vignetteMaterial.SetFloat("_RedIntensity", redVignetteIntensity);
            vignetteMaterial.SetFloat("_BlueIntensity", blueVignetteIntensity);
            Graphics.Blit(temp, destination, vignetteMaterial);
        }
        else
        {
            Graphics.Blit(temp, destination);
        }
        
        RenderTexture.ReleaseTemporary(temp);
    }
}