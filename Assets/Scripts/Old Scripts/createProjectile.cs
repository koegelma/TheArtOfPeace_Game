/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class createProjectile : MonoBehaviour
{
    public Rigidbody projectile;
    public GameObject leftController, rightController;
    public GameObject xrRig;
    public InputDevice leftHandDevice;
    // Start is called before the first frame update
    public float triggerValue;
    public float timeStamp = 0;
    public bool isCapturing;
    public List<Vector3> vectorList;


    void Start()
    {
        vectorList = new List<Vector3>();
        xrRig = GameObject.Find("XRRig");
        leftController = GameObject.Find("LeftController");
        leftHandDevice = xrRig.GetComponent<OutputInput>().getDevice();
    }

    void Update()
    {
        if (!isCapturing && Time.time % 0.1 <= 0.1)
        {
            checkTrigger();
            return;
        }
        float coolDownPeriodInSeconds = 2f;
        if (timeStamp <= Time.time && triggerValue == 0 && isCapturing)
        {
            shootProjectile();
            timeStamp = Time.time + coolDownPeriodInSeconds;
        }
        isCapturing = false;
    }

    public void checkTrigger()
    {
        var leftHanded = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, leftHanded);

        foreach (var device in leftHanded)
        {
            leftHandDevice = device;
        }

        if (leftHandDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue) && triggerValue >= 0.1)
        {
            isCapturing = true;
            Vector3 controllerAcceleration;
            if (leftHandDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out controllerAcceleration)) ;
            vectorList.Add(controllerAcceleration);
            Debug.Log("Controller Acceleration: X: " + controllerAcceleration.x + "   Y: " + controllerAcceleration.y + "   Z: " + controllerAcceleration.z);
        }
    }
    void shootProjectile()
    {
        Vector3 sum = new Vector3(0, 0, 0);
        for (int i = 0; i < vectorList.Count; i++)
        {
            sum += vectorList[i];
        }
        Vector3 result = new Vector3(sum.x / vectorList.Count, sum.y / vectorList.Count, sum.z / vectorList.Count);
        Rigidbody projectileInstance = Instantiate(projectile, leftController.transform.position, Quaternion.identity) as Rigidbody;
        projectileInstance.AddForce(result * 10); //leftController.transform.forward * 

    }
}


 */