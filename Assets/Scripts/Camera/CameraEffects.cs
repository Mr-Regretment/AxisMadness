using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraEffects : MonoBehaviour
{
    [Header("Glitch")]
    [SerializeField] private Material glitchMaterial;
    [SerializeField] private CameraHandler cameraHandler;
    [SerializeField] private float glitchIntensity = 1.0f;
    [SerializeField] private float glitchDuration = 1f;

    [Header("Vignette")]
    [SerializeField] private float redVignetteIntensity = 0.3f;
    [SerializeField] private float blueVignetteIntensity = 0.3f;

    [Header("Other Effects")]
    [SerializeField] private Material posterizeMaterial;
    [SerializeField][Range(1, 32)] private float steps = 8f;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material colourGradingMaterial;
    [SerializeField][Range(0, 1)] private float colourGradingStrength = 1f;

    private Material vignetteMaterial;
    private bool shouldGlitch = false;
    private float glitchTimer = 0f;
    private bool wasRotating = false;

    private void Start()
    {
        vignetteMaterial = new Material(Shader.Find("Hidden/Vignette"));
    }

    private void Update()
    {
        if (cameraHandler != null)
        {
            bool isRotating = cameraHandler.IsRotatingAnimation();
            if (isRotating && !wasRotating)
                TriggerGlitch();
            wasRotating = isRotating;
        }

        if (glitchTimer > 0f)
        {
            glitchTimer -= Time.deltaTime;
            if (glitchTimer <= 0f)
            {
                glitchTimer = 0f;
                shouldGlitch = false;
            }
        }

        if (shouldGlitch && glitchMaterial != null)
        {
            glitchMaterial.SetVector("_ScanLineJitter", new Vector2(1.0f * glitchIntensity, 0.1f));
            glitchMaterial.SetVector("_VerticalJump", new Vector2(0.2f * glitchIntensity, Time.time));
            glitchMaterial.SetFloat("_HorizontalShake", 1.0f * glitchIntensity);
            glitchMaterial.SetVector("_ColorDrift", new Vector2(1.0f * glitchIntensity, Time.time));
        }
    }

    public void TriggerGlitch()
    {
        shouldGlitch = true;
        glitchTimer = glitchDuration;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture current = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, current);

        if (outlineMaterial != null)
        {
            RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(current, temp, outlineMaterial);
            RenderTexture.ReleaseTemporary(current);
            current = temp;
        }

        if (posterizeMaterial != null)
        {
            posterizeMaterial.SetFloat("_Steps", steps);
            RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(current, temp, posterizeMaterial);
            RenderTexture.ReleaseTemporary(current);
            current = temp;
        }

        if (colourGradingMaterial != null)
        {
            colourGradingMaterial.SetFloat("_Strength", colourGradingStrength);
            RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(current, temp, colourGradingMaterial);
            RenderTexture.ReleaseTemporary(current);
            current = temp;
        }

        if (shouldGlitch && glitchMaterial != null)
        {
            RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(current, temp, glitchMaterial);
            RenderTexture.ReleaseTemporary(current);
            current = temp;
        }

        if (shouldGlitch && vignetteMaterial != null)
        {
            vignetteMaterial.SetFloat("_RedIntensity", redVignetteIntensity);
            vignetteMaterial.SetFloat("_BlueIntensity", blueVignetteIntensity);
            RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(current, temp, vignetteMaterial);
            RenderTexture.ReleaseTemporary(current);
            current = temp;
        }

        Graphics.Blit(current, destination);
        RenderTexture.ReleaseTemporary(current);
    }
}