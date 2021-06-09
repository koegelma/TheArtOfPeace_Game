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
    public GameObject XRRig;
    public GameObject fireballPrefab;
    public float fireballCooldownInSeconds = 2;
    public float fireballTimestamp = 0;
    public State state = State.IDLE;
    public float stateTimestamp;
    public GameObject cubeLeft;
    public GameObject cubeRight;
    public GameObject manaCube;
    public int currentPhase = -1;
    public Controller left;
    public Controller right;


    void Update()
    {
        updateStates();
        updateTestCubes();
        if(left.isTrigger && right.isTrigger){
            checkStartingPoints();
        }
    }
    public void checkStartingPoints()
    {
        float tolerance = 0.2f;

        if ((this.GetComponent<Mana>().currentMana >= Fireball.manaCost && fireballTimestamp <= Time.time && state == State.IDLE) || state == State.FIREBALL)
        {
            if (Fireball.checkPhase(left.relativeTransform, right.relativeTransform, tolerance, 0) || state == State.FIREBALL)
            {
                state = State.FIREBALL;
                stateTimestamp = Time.time;
                if (Fireball.checkPhase(left.relativeTransform, right.relativeTransform, tolerance, 1))
                {
                    this.GetComponent<Mana>().currentMana -= Fireball.manaCost;
                    GameObject fireballInstance = Instantiate(fireballPrefab, right.controllerPosition, right.controllerRotation);
                    fireballInstance.GetComponent<Rigidbody>().AddForce(fireballInstance.transform.forward * 80);
                    fireballTimestamp = Time.time + fireballCooldownInSeconds;
                    state = State.IDLE;
                }
            }
        }

        if (state == State.IDLE || state == State.MEDITATE)
        {
            float velocityLeft = left.controllerVelocity.magnitude;
            float velocityRight = right.controllerVelocity.magnitude;
            if (Meditate.isFluid)
            {
                //play error sound and animation
                currentPhase = -1;
                state = State.IDLE;
                Meditate.isFluid = false;
            }
            else if (Meditate.checkPhase(left.relativeTransform, right.relativeTransform, velocityLeft, velocityRight, tolerance, 0)
            || (state == State.MEDITATE && currentPhase >= 0))
            {
                state = State.MEDITATE;
                stateTimestamp = Time.time;
                currentPhase = assurePhase(0);
                if ((Meditate.checkPhase(left.relativeTransform, right.relativeTransform, velocityLeft, velocityRight, tolerance, 1) && currentPhase == 0)
                || (state == State.MEDITATE && currentPhase > 0))
                {
                    currentPhase = assurePhase(1);
                    if (Meditate.checkPhase(left.relativeTransform, right.relativeTransform, velocityLeft, velocityRight, 0.1f, 2) && currentPhase == 1)
                    {
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
    public int assurePhase(int phaseGiven)
    {
        if (phaseGiven > currentPhase)
        {
            return phaseGiven;
        }
        return currentPhase;
    }
    public void updateStates()
    {
        if ((stateTimestamp < Time.time - 5 && state != State.IDLE) || (left.isTrigger && right.isTrigger) != true)
        {
            state = State.IDLE;
            currentPhase = -1;
        }
    }
    public void updateTestCubes()
    {

        float xscale = GetComponent<Mana>().currentMana / 50;
        if (state == State.FIREBALL)
        {
            cubeLeft.transform.localScale = new Vector3(0.5f, 1, 0.5f);
            cubeRight.transform.localScale = new Vector3(0.5f, 1, 0.5f);
        }
        manaCube.transform.localScale = new Vector3(xscale, 0.2f, 0.2f);
        if (state == State.MEDITATE)
        {
            manaCube.transform.localScale = new Vector3(xscale, 1, 0.2f);
        }
        else
        {
            manaCube.transform.localScale = new Vector3(xscale, 0.2f, 0.2f);
        }
        if (state == State.IDLE)
        {
            cubeLeft.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            cubeRight.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}