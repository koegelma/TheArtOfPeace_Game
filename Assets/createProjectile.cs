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
    int count;
    private Vector3[] vectorArray;
    void Start()
    {
        xrRig = GameObject.Find("XRRig");
        leftController = GameObject.Find("LeftController");
        leftHandDevice = xrRig.GetComponent<OutputInput>().getDevice();
    }

    // Update is called once per frame
 
    void Update()
    {
        checkTrigger();
    }

    void shootProjectile()
    {
        Vector3 controllerAcceleration;
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out controllerAcceleration));
        Rigidbody projectileInstance;
        projectileInstance = Instantiate(projectile, leftController.transform.position, Quaternion.identity) as Rigidbody;
        projectileInstance.AddForce(controllerAcceleration * 10 ); //leftController.transform.forward * 
    }

    public async void checkTrigger()
    {
        var leftHanded = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, leftHanded);

        foreach (var device in leftHanded)
        {
            leftHandDevice = device;
        }

        if (leftController.transform.position.y > 0.5f)
        {
            //Debug.Log("Shut 1");

            //Problem - no trigger value, was da los?
            leftHandDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);

            if (leftController.transform.position.y > 0.5f)
            {

                // Coroutines? Ehre
                while (leftHandDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue) && triggerValue >= 0.1)
                {
                    Vector3 controllerAcceleration;
                    if (leftHandDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out controllerAcceleration)) ;
                    vectorArray[count] = controllerAcceleration;

                    //for(int i=0;)


                    count++;
                }
                count = 0;
                // wird nicht mehr aufgerufen, da while nur abbricht, wenn das if nicht mehr true ist
                if ((leftHandDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue) && triggerValue >= 0.1))
                {
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
}
