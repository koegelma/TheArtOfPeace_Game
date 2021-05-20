//using System.Diagnostics;
//using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum State
{
    IDLE, FIREBALL
}
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
    public GameObject fireballPrefab;
    public float fireballCooldownInSeconds = 2;
    public float fireballTimestamp = 0;
    public State state = State.IDLE;
    public float stateTimestamp;
    public GameObject cubeLeft;
    public GameObject cubeRight;

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        registerDevices();
        updateStates();
        
        if(state == State.FIREBALL){
            cubeLeft.transform.localScale = new Vector3(0.5f,1,0.5f);
            cubeRight.transform.localScale = new Vector3(0.5f,1,0.5f);
        }
        else{
            cubeLeft.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
            cubeRight.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        }
        //Debug.Log(state.ToString());
        
        if (checkTrigger())
        {
            checkStartingPoints();
            //Debug.Log("Left: " + relativeControllerTransformLeft.transform.position + "      Right: " + relativeControllerTransformRight.transform.position);
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
        float tolerance = 0.2f;
        if (fireballTimestamp <= Time.time || state == State.FIREBALL)
        {
            if (Fireball.checkPhase(relativeControllerTransformLeft, relativeControllerTransformRight, tolerance, 0) || state == State.FIREBALL)
            {
                state = State.FIREBALL;
                stateTimestamp = Time.time;
                if (Fireball.checkPhase(relativeControllerTransformLeft, relativeControllerTransformRight, tolerance, 1))
                {
                    GameObject fireballInstance = Instantiate(fireballPrefab, controllerPositionRight, controllerRotationRight);
                    fireballInstance.GetComponent<Rigidbody>().AddForce(fireballInstance.transform.forward * 80);
                    fireballTimestamp = Time.time + fireballCooldownInSeconds;
                    state = State.IDLE;
                }
            }
        }
    }
    public void updateStates(){
        if ((stateTimestamp < Time.time - 5 && state != State.IDLE) || checkTrigger() != true)
        {
            state = State.IDLE;
        }
    }
    public void registerDevices()
    {
        if (Time.time % 1 <= 0.1)
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
    }

    public void updateRelativeControllerPositionAndRotation()
    {
        leftHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out controllerPositionLeft);
        rightHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out controllerPositionRight);
        leftHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out controllerRotationLeft);
        rightHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out controllerRotationRight);

        controllerRotationLeft *= Quaternion.Euler(Vector3.right * 20);
        controllerRotationRight *= Quaternion.Euler(Vector3.right * 20);

        relativeControllerTransformLeft.transform.position = controllerPositionLeft - mainCamera.transform.position;
        relativeControllerTransformRight.transform.position = controllerPositionRight - mainCamera.transform.position;

        Vector3 anglesLeft = controllerRotationLeft.eulerAngles - mainCamera.transform.rotation.eulerAngles;
        temporaryRotationLeft = Quaternion.Euler(anglesLeft);
        Vector3 anglesRight = controllerRotationRight.eulerAngles - mainCamera.transform.rotation.eulerAngles;
        temporaryRotationRight = Quaternion.Euler(anglesRight);

        relativeControllerTransformLeft.transform.RotateAround(XRRig.transform.position, new Vector3(0, 1, 0), -mainCamera.transform.rotation.eulerAngles.y);
        relativeControllerTransformRight.transform.RotateAround(XRRig.transform.position, new Vector3(0, 1, 0), -mainCamera.transform.rotation.eulerAngles.y);
        relativeControllerTransformLeft.transform.rotation = temporaryRotationLeft;
        relativeControllerTransformRight.transform.rotation = temporaryRotationRight;

        /* cubeLeft.transform.position = relativeControllerTransformLeft.transform.position;
        cubeRight.transform.position = relativeControllerTransformRight.transform.position;
        cubeLeft.transform.rotation = relativeControllerTransformLeft.transform.rotation;
        cubeRight.transform.rotation = relativeControllerTransformRight.transform.rotation; */
    }
}