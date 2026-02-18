using TMPro;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [Header("Tutorial")]
    
    [SerializeField] GameObject player;
    [SerializeField] GameObject startingText;

    [SerializeField] private GameObject cameraHandler;
    
    private bool _startedTutorial;
    private bool _isInTutorialBeginning;
    
    private Vector3 targetPosition;
    private Vector3 startingPosition;

    void Start()
    {
        if (startingText != null)
        {
            startingPosition = startingText.transform.position;
            targetPosition = startingText.transform.position + Vector3.down * 175;
        }
        if (player != null)
        {
            _startedTutorial = true;
            _isInTutorialBeginning = true;
        }
    }

    void Update()
    {
        if (_startedTutorial)
        {
            
            startingText.transform.position = Vector3.Lerp(startingText.transform.position, targetPosition, Time.deltaTime * 4f);
        }
    }
    
    
    
}
