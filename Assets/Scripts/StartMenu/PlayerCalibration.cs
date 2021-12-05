using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCalibration : MonoBehaviour
{
    private float standardArmLength = 1.6f;
    private float playerArmLength;
    private Controller leftController;
    private Controller rightController;
    private bool isCalibrationInputs = true;
    public PlayerSettings playerSettingsScript;
    private void Start()
    {
        leftController = GameObject.Find("Left Controller").GetComponent<Controller>();
        rightController = GameObject.Find("Right Controller").GetComponent<Controller>();
    }

    private void Update()
    {
        if (leftController.isGrip && rightController.isGrip && isCalibrationInputs)
        {
            CalibrateArmLength();
            isCalibrationInputs = false;
        }
        if (!isCalibrationInputs && !leftController.isGrip && !rightController.isGrip) isCalibrationInputs = true;
    }

    private void CalibrateArmLength()
    {
        playerArmLength = Vector3.Distance(leftController.controllerPosition, rightController.controllerPosition);
        Debug.Log(playerArmLength);
        GameData.playerArmLength = playerArmLength;
        GameData.isPlayerInitialized = true;
        playerSettingsScript.ToggleCalibration();
    }
}
