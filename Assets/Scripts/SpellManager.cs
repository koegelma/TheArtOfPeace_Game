using System.Globalization;
//using System.Diagnostics;
//using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum State
{
    IDLE, FIREBALL, MEDITATE
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
    public Vector3 controllerVelocityLeft;
    public Vector3 controllerVelocityRight;
    public GameObject cubeLeft;
    public GameObject cubeRight;
    public GameObject manaCube;
    public int currentPhase = -1;
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        registerDevices();
        updateStates();
        
        updateTestCubes();
        /* if(state != State.IDLE){
            Debug.Log(state.ToString());
        } */
        
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

        if ((this.GetComponent<Mana>().currentMana >= Fireball.manaCost && fireballTimestamp <= Time.time && state == State.IDLE) || state == State.FIREBALL)
        {
            if (Fireball.checkPhase(relativeControllerTransformLeft, relativeControllerTransformRight, tolerance, 0) || state == State.FIREBALL)
            {
                state = State.FIREBALL;
                stateTimestamp = Time.time;
                if (Fireball.checkPhase(relativeControllerTransformLeft, relativeControllerTransformRight, tolerance, 1))
                {
                    this.GetComponent<Mana>().currentMana -= Fireball.manaCost;
                    GameObject fireballInstance = Instantiate(fireballPrefab, controllerPositionRight, controllerRotationRight);
                    fireballInstance.GetComponent<Rigidbody>().AddForce(fireballInstance.transform.forward * 80);
                    fireballTimestamp = Time.time + fireballCooldownInSeconds;
                    state = State.IDLE;
                }
            }
        }

        if (state == State.IDLE || state == State.MEDITATE)
        {
            float velocityLeft = controllerVelocityLeft.magnitude;
            float velocityRight = controllerVelocityRight.magnitude;
            if(Meditate.isTooFastAndTooFurious7){
                //play error sound and animation
                currentPhase=-1;
                state = State.IDLE;
                Meditate.isTooFastAndTooFurious7 = false;
            }
            else if (Meditate.checkPhase(relativeControllerTransformLeft, relativeControllerTransformRight, velocityLeft, velocityRight, tolerance, 0)
            || (state == State.MEDITATE && currentPhase >= 0))
            {
                state = State.MEDITATE;
                stateTimestamp = Time.time;
                currentPhase = assurePhase(0);
                if((Meditate.checkPhase(relativeControllerTransformLeft, relativeControllerTransformRight, velocityLeft, velocityRight, tolerance, 1) && currentPhase == 0)
                || (state == State.MEDITATE && currentPhase > 0))
                {
                    currentPhase = assurePhase(1);
                    if(Meditate.checkPhase(relativeControllerTransformLeft, relativeControllerTransformRight, velocityLeft, velocityRight, 0.1f, 2) && currentPhase == 1){
                        XRRig.AddComponent<ManaRegeneration>();
                        XRRig.GetComponent<ManaRegeneration>().duration = 5;
                        XRRig.GetComponent<ManaRegeneration>().strength = 20;
                        state = State.IDLE;
                        currentPhase = -1;
                    }
                }
            }
        }
    }
    public int assurePhase(int phaseGiven){
        if(phaseGiven > currentPhase){
            return phaseGiven;
        }
        return currentPhase;
    }
    public void updateStates(){
        if ((stateTimestamp < Time.time - 5 && state != State.IDLE) || checkTrigger() != true)
        {
            state = State.IDLE;
            currentPhase = -1;
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
        leftHandDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out controllerVelocityLeft);
        rightHandDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out controllerVelocityRight);

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

        
    }

    public void updateTestCubes(){
        /* cubeLeft.transform.position = relativeControllerTransformLeft.transform.position;
        cubeRight.transform.position = relativeControllerTransformRight.transform.position;
        cubeLeft.transform.rotation = relativeControllerTransformLeft.transform.rotation;
        cubeRight.transform.rotation = relativeControllerTransformRight.transform.rotation; */
        float xscale = GetComponent<Mana>().currentMana/50;
        if(state == State.FIREBALL){
            cubeLeft.transform.localScale = new Vector3(0.5f,1,0.5f);
            cubeRight.transform.localScale = new Vector3(0.5f,1,0.5f);
        }
        manaCube.transform.localScale = new Vector3(xscale, 0.2f, 0.2f);
        if(state == State.MEDITATE){
            manaCube.transform.localScale = new Vector3(xscale, 1, 0.2f);
        }
        else{
            manaCube.transform.localScale = new Vector3(xscale, 0.2f, 0.2f);
        }
        if(state == State.IDLE){
            cubeLeft.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
            cubeRight.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        }
        
        
    }
}