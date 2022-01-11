using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Controller : MonoBehaviour
{
    public Vector3 controllerPosition;
    public Quaternion controllerRotation;
    public Transform relativeTransform;
    public InputDevice device;
    public GameObject XRRig;
    public Quaternion temporaryRotation;
    public Vector3 controllerVelocity;
    public Vector3 controllerAcceleration;
    public GameObject mainCamera;
    public float triggerValue;
    public bool isTrigger;
    public float gripValue;
    public bool isGrip;
    public bool isMenuButton;
    public bool isPrimaryButton;
    public Transform nextRelativeTransform;
    //public GameObject nextRelativeTransformGO;

    void Update()
    {
        registerDevice();
        CheckControllerInput();
        UpdateFirstRelativeControllerTransform();
        UpdateNextRelativeControllerTransform();
    }
    public void registerDevice()
    {
        if (Time.time % 1 <= 0.1)
        {
            var leftHanded = new List<UnityEngine.XR.InputDevice>();
            var rightHanded = new List<UnityEngine.XR.InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, leftHanded);
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, rightHanded);
            if (this.gameObject.name.Equals("Left Controller"))
            {
                foreach (var devices in leftHanded)
                {
                    this.device = devices;
                }
            }
            else if (this.gameObject.name.Equals("Right Controller"))
            {
                foreach (var devices in rightHanded)
                {
                    this.device = devices;
                }
            }
        }
    }

    private void CheckControllerInput()
    {
        device.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);
        CheckTrigger();
        device.TryGetFeatureValue(CommonUsages.grip, out gripValue);
        CheckGrip();
        device.TryGetFeatureValue(CommonUsages.devicePosition, out controllerPosition);
        device.TryGetFeatureValue(CommonUsages.deviceRotation, out controllerRotation);
        device.TryGetFeatureValue(CommonUsages.deviceVelocity, out controllerVelocity);
        device.TryGetFeatureValue(CommonUsages.deviceAcceleration, out controllerAcceleration);
        device.TryGetFeatureValue(CommonUsages.menuButton, out isMenuButton);
        device.TryGetFeatureValue(CommonUsages.primaryButton, out isPrimaryButton);
    }
    public void UpdateFirstRelativeControllerTransform()
    {
        controllerRotation *= Quaternion.Euler(Vector3.right * 20);

        relativeTransform.position = controllerPosition - mainCamera.transform.position;

        Vector3 angles = controllerRotation.eulerAngles - mainCamera.transform.rotation.eulerAngles;
        temporaryRotation = Quaternion.Euler(angles);

        relativeTransform.RotateAround(XRRig.transform.position, new Vector3(0, 1, 0), -mainCamera.transform.rotation.eulerAngles.y);
        relativeTransform.rotation = temporaryRotation;
    }

    public GameObject GetMainCamera()
    {
        return mainCamera;
    }

    public void UpdateNextRelativeControllerTransform()
    {
        nextRelativeTransform.position = controllerPosition - mainCamera.transform.position;

        //Vector3 relativeAddedHeight = nextRelativeTransform.position + mainCamera.transform.position;

        //nextRelativeTransformGO.transform.position = relativeAddedHeight;

        //nextRelativeTransformGO.transform.position = new Vector3(nextRelativeTransformGO.transform.position.x, nextRelativeTransformGO.transform.position.y + mainCamera.transform.position.y, nextRelativeTransformGO.transform.position.z);

    }

    void CheckTrigger()
    {
        if (triggerValue >= 0.1) isTrigger = true;
        else isTrigger = false;
    }
    void CheckGrip()
    {
        if (gripValue >= 0.1) isGrip = true;
        else isGrip = false;
    }
}
