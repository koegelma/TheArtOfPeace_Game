using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ShootProjectile : MonoBehaviour
{
    //public UnityEngine.XR.XRNode leftHand = UnityEngine.XR.XRNode.LeftHand;
    public GameObject leftController, rightController;
    public bool triggerValue;
    public InputDevice device;
    public InputDevice leftHandDevice;
    public bool isLeftHand = true;
    public Vector3 test;
    public Vector3 leftPosition;
    public Quaternion leftRotation;

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (isLeftHand)
        {
            var leftHanded = new List<UnityEngine.XR.InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, leftHanded);

            foreach (var device in leftHanded)
            {
                leftHandDevice = device;
            }
            isLeftHand = false;
        }
        bool triggerValue;
        if (leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
        {
            
        }

        leftHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out leftPosition);
        leftHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out leftRotation);
        Debug.Log("position: " + leftPosition + "   rotation: " + leftRotation);

    }
}

