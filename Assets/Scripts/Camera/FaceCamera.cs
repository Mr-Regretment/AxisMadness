using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    private TextMeshProUGUI text;
    private Image panelImage;
    private Transform player;
    [SerializeField] private GameObject objectTransform;
    private float fadeDistance = 5f;

    private void Start()
    {
        mainCamera = Camera.main;
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
        panelImage = GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180, 0);
        
        float distance = 0f;
        if (objectTransform != null)
            distance = Vector3.Distance(objectTransform.transform.position, player.position);
        
        float alpha = Mathf.Clamp01(1f - (distance / fadeDistance));
        
        Color color = text.color;
        color.a = alpha;
        text.color = color;
        
        if (panelImage != null)
        {
            Color panelColor = panelImage.color;
            panelColor.a = alpha;
            panelImage.color = panelColor;
        }
        
        
    }
}