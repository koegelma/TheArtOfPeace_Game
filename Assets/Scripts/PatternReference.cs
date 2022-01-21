using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternReference : MonoBehaviour
{
    public static PatternReference instance;
    public GameObject leftHelper;
    public GameObject rightHelper;
    public GameObject leftChild;
    public GameObject rightChild;
    public Controller leftController;
    public Controller rightController;
    public Transform cameraTransform;
    public GameObject secondNextRightPhaseCoord;
    public GameObject secondNextLeftPhaseCoord;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PatternReference in scene!");
            return;
        }
        instance = this;
    }

    public void GetSceneReferences(Pattern _pattern)
    {
        _pattern.leftHelper = this.leftHelper;
        _pattern.rightHelper = this.rightHelper;
        _pattern.leftChild = this.leftChild;
        _pattern.rightChild = this.rightChild;
        _pattern.leftController = this.leftController;
        _pattern.rightController = this.rightController;
        _pattern.cameraTransform = this.cameraTransform;
        _pattern.secondNextLeftPhaseCoord = this.secondNextLeftPhaseCoord;
        _pattern.secondNextRightPhaseCoord = this.secondNextRightPhaseCoord;
    }
}
