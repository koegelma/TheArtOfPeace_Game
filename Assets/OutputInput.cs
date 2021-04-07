using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OutputInput : MonoBehaviour
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
    public GameObject projectile;

    void Start()
    {
        projectile = GameObject.Find("Projectile");
        leftController = GameObject.Find("LeftController");
        rightController = GameObject.Find("RightController");

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
        }


        leftHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out leftPosition);
        leftHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out leftRotation);
        //Debug.Log("position: " + leftPosition + "   rotation: " + leftRotation);
    }

    public InputDevice getDevice()
    {
        return leftHandDevice;
    }
}


    
