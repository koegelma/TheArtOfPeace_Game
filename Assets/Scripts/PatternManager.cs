using UnityEngine;

public class PatternManager : MonoBehaviour
{
    public static PatternManager instance;
    public Controller leftController;
    public Controller rightController;
    private Pattern[] patterns;
    private int currentPatternIndex;
    private bool isLeftGripReady = true;
    private bool isRightGripReady = true;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PatternManager in scene!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        patterns = new Pattern[transform.childCount];
        foreach (Transform child in transform)
        {
            patterns[child.GetSiblingIndex()] = child.GetComponent<Pattern>();
            Debug.Log(patterns[child.GetSiblingIndex()].pattern);
        }
        patterns[0].isSelected = true;
        currentPatternIndex = 0;
    }

    private void Update()
    {
        if (!leftController.isGrip && !isLeftGripReady) isLeftGripReady = true;
        if (!rightController.isGrip && !isRightGripReady) isRightGripReady = true;
        if (leftController.isGrip && isLeftGripReady)
        {
            if (currentPatternIndex > 0)
            {
                patterns[currentPatternIndex].isSelected = false;
                currentPatternIndex--;
                patterns[currentPatternIndex].isSelected = true;
            }
            isLeftGripReady = false;
        }
        if (rightController.isGrip && isRightGripReady)
        {
            if (currentPatternIndex < patterns.Length - 1)
            {
                patterns[currentPatternIndex].isSelected = false;
                currentPatternIndex++;
                patterns[currentPatternIndex].isSelected = true;
            }
            isRightGripReady = false;
        }
    }
}
