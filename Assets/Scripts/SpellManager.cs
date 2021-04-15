//using System.Diagnostics;
//using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SpellManager : MonoBehaviour
{
    public GameObject controllerLeft, controllerRight;
    public GameObject xrRig;
    public GameObject mainCamera;
    public InputDevice leftHandDevice;
    public InputDevice rightHandDevice;
    public float triggerValueLeft;
    public float triggerValueRight;
    public List<Vector3> vectorList;
    public Vector3 controllerPositionLeft;
    public Vector3 controllerPositionRight;
    public Vector3 relativeControllerPositionLeft;
    public Vector3 relativeControllerPositionRight;
    public Quaternion relativeControllerRotationLeft;
    public Quaternion relativeControllerRotationRight;
    public Quaternion controllerRotationLeft;
    public Quaternion controllerRotationRight;

    public GameObject cubeLeft;

    public GameObject cubeRight;


    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time % 1 <= 0.1){
            registerDevices();
        }
        
        if(checkTrigger()){
            checkStartingPoints();
        }
    }
    public bool checkTrigger()
    {
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValueLeft) && triggerValueLeft >= 0.1 && 
        rightHandDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValueRight) && triggerValueRight >= 0.1)
        {
            return true;
        }
        return false;
    }

    public void checkStartingPoints()
    {
        updateRelativeControllerPositionAndRotation();
        float tolerance = 0.1f;
        if(relativeControllerPositionLeft.x < Fireball.startingPointLeft.x + tolerance && relativeControllerPositionLeft.x > Fireball.startingPointLeft.x - tolerance 
        && relativeControllerPositionLeft.y < Fireball.startingPointLeft.y + tolerance && relativeControllerPositionLeft.y > Fireball.startingPointLeft.y - tolerance
        && relativeControllerPositionLeft.z < Fireball.startingPointLeft.z + tolerance && relativeControllerPositionLeft.z > Fireball.startingPointLeft.z - tolerance)
        {
            Fireball fireball = new Fireball(relativeControllerPositionRight);
        }
    }
    public void registerDevices()
    {
        var leftHanded = new List<UnityEngine.XR.InputDevice>();
        var rightHanded = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, leftHanded);
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, rightHanded);
        foreach (var device in leftHanded)
        {
            leftHandDevice = device;
        }
        foreach (var device in rightHanded)
        {
            rightHandDevice = device;
        }
    }

    public void updateRelativeControllerPositionAndRotation(){
        leftHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out controllerPositionLeft);
        rightHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out controllerPositionRight);
        leftHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out controllerRotationLeft);
        rightHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out controllerRotationRight);

        relativeControllerPositionLeft = controllerPositionLeft - mainCamera.transform.position;
        relativeControllerPositionRight = controllerPositionRight - mainCamera.transform.position;
        Debug.Log("Left: " + relativeControllerPositionLeft + "      Right: " + relativeControllerPositionRight);

        Vector3 anglesLeft = controllerRotationLeft.eulerAngles - mainCamera.transform.rotation.eulerAngles;
        relativeControllerRotationLeft = Quaternion.Euler(anglesLeft);
        Vector3 anglesRight = controllerRotationRight.eulerAngles - mainCamera.transform.rotation.eulerAngles;
        relativeControllerRotationRight = Quaternion.Euler(anglesRight);

        cubeLeft.transform.position = relativeControllerPositionLeft;
        cubeRight.transform.position = relativeControllerPositionRight;
        cubeLeft.transform.rotation = relativeControllerRotationLeft;
        cubeRight.transform.rotation = relativeControllerRotationRight;
        // The relative controller positions are not relative to the rotation of the headset. Rotation of the headset has to be part of the equation.
    }
}
