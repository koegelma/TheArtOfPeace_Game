using System.Collections;
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
    int count = 0;
    
    void Start()
    {
        vectorList = new List<Vector3>();
        xrRig = GameObject.Find("XRRig");
        leftController = GameObject.Find("LeftController");
        leftHandDevice = xrRig.GetComponent<OutputInput>().getDevice();
    }

    // Update is called once per frame
 
    void Update()
    {
        if (!isCapturing)
        {
            checkTrigger();
        }
    }

    void shootProjectile()
    {
        Vector3 controllerAcceleration;
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out controllerAcceleration));
        Rigidbody projectileInstance;
        projectileInstance = Instantiate(projectile, leftController.transform.position, Quaternion.identity) as Rigidbody;
        projectileInstance.AddForce(controllerAcceleration * 10 ); //leftController.transform.forward * 
    }

    public void checkTrigger()
    {
        var leftHanded = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, leftHanded);

        foreach (var device in leftHanded)
        {
            leftHandDevice = device;
        }

       
            //Debug.Log("Shut 1");

            //Problem - no trigger value, was da los?
            leftHandDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);

            if (leftController.transform.position.y > 0.5f)
            {
                if (leftHandDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue) && triggerValue >= 0.1)
                {  
                    while (leftHandDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue) && triggerValue >= 0.1)
                    {
                        isCapturing = true;
                        Vector3 controllerAcceleration;
                        if (leftHandDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out controllerAcceleration));
                        vectorList.Add(controllerAcceleration);
                        count++;
                    }
                    isCapturing = false;
                    float coolDownPeriodInSeconds = 2f;
                    if (timeStamp <= Time.time)
                    {
                    shootProjectile();
                    timeStamp = Time.time + coolDownPeriodInSeconds;
                    }
                }
            }
    }
}
    

