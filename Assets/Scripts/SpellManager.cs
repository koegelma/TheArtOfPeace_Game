//using System.Diagnostics;
//using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SpellManager : MonoBehaviour
{
    public GameObject mainCamera;
    public InputDevice leftHandDevice;
    public InputDevice rightHandDevice;
    public float triggerValueLeft;
    public float triggerValueRight;
    //public List<Vector3> vectorList;
    public Vector3 controllerPositionLeft;
    public Vector3 controllerPositionRight;
    public Quaternion controllerRotationLeft;
    public Quaternion controllerRotationRight;
    public GameObject relativeControllerTransformLeft;
    public GameObject relativeControllerTransformRight;
    public GameObject XRRig;
    public Quaternion temporaryRotationLeft;
    public Quaternion temporaryRotationRight;

    public GameObject spells;
    //public GameObject cubeLeft;
    //public GameObject cubeRight;
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
            Debug.Log("Left: " + relativeControllerTransformLeft.transform.position + "      Right: " + relativeControllerTransformRight.transform.position);
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
        if((relativeControllerTransformLeft.transform.position.x < Fireball.startingPointLeft.x + tolerance && relativeControllerTransformLeft.transform.position.x > Fireball.startingPointLeft.x - tolerance 
        && relativeControllerTransformLeft.transform.position.y < Fireball.startingPointLeft.y + tolerance && relativeControllerTransformLeft.transform.position.y > Fireball.startingPointLeft.y - tolerance
        && relativeControllerTransformLeft.transform.position.z < Fireball.startingPointLeft.z + tolerance && relativeControllerTransformLeft.transform.position.z > Fireball.startingPointLeft.z - tolerance)
        && // Relative position of the Left&Right controller have to be on startingPoint +- 0.1
           (relativeControllerTransformRight.transform.position.x < Fireball.startingPointRight.x + tolerance && relativeControllerTransformRight.transform.position.x > Fireball.startingPointRight.x - tolerance 
        && relativeControllerTransformRight.transform.position.y < Fireball.startingPointRight.y + tolerance && relativeControllerTransformRight.transform.position.y > Fireball.startingPointRight.y - tolerance
        && relativeControllerTransformRight.transform.position.z < Fireball.startingPointRight.z + tolerance && relativeControllerTransformRight.transform.position.z > Fireball.startingPointRight.z - tolerance))
        {
            Fireball fireball = spells.AddComponent(typeof(Fireball)) as Fireball;
            fireball.run(relativeControllerTransformRight.transform.position);
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

        relativeControllerTransformLeft.transform.position = controllerPositionLeft - mainCamera.transform.position;
        relativeControllerTransformRight.transform.position = controllerPositionRight - mainCamera.transform.position;

        Vector3 anglesLeft = controllerRotationLeft.eulerAngles - mainCamera.transform.rotation.eulerAngles;
        temporaryRotationLeft = Quaternion.Euler(anglesLeft);
        Vector3 anglesRight = controllerRotationRight.eulerAngles - mainCamera.transform.rotation.eulerAngles;
        temporaryRotationRight = Quaternion.Euler(anglesRight);

        relativeControllerTransformLeft.transform.RotateAround(XRRig.transform.position, new Vector3(0,1,0), -mainCamera.transform.rotation.eulerAngles.y);
        relativeControllerTransformRight.transform.RotateAround(XRRig.transform.position, new Vector3(0,1,0), -mainCamera.transform.rotation.eulerAngles.y);
        relativeControllerTransformLeft.transform.rotation = temporaryRotationLeft;
        relativeControllerTransformRight.transform.rotation = temporaryRotationRight;

        /* cubeLeft.transform.position = relativeControllerTransformLeft.transform.position;
        cubeRight.transform.position = relativeControllerTransformRight.transform.position;
        cubeLeft.transform.rotation = relativeControllerTransformLeft.transform.rotation;
        cubeRight.transform.rotation = relativeControllerTransformRight.transform.rotation; */
    }
}
