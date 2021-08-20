using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern1 : MonoBehaviour
{
    public PhaseChecker phaseChecker;
    public Vector3[] leftPhaseCoords;
    public Vector3[] rightPhaseCoords;
    public GameObject leftHelper;
    public GameObject rightHelper;
    public Controller leftController;
    public Controller rightController;
    public Transform cameraTransform;
    private Vector3 offset = new Vector3(0, 0, 2);
    /*
    void testPosition(){
        if (leftController.isTrigger && rightController.isTrigger){
            Debug.Log("left: "+leftController.relativeTransform.position);
            Debug.Log("right: "+rightController.relativeTransform.position);
        }
    } */
    void Awake()
    {
        leftPhaseCoords = new Vector3[3];
        rightPhaseCoords = new Vector3[3];
        leftPhaseCoords[0] = new Vector3(-0.2f, -0.1f, 0.6f);
        rightPhaseCoords[0] = new Vector3(0.1f, -0.3f, 0.3f);
        leftPhaseCoords[1] = new Vector3(-0.1f, -0.2f, 0.2f);
        rightPhaseCoords[1] = new Vector3(0.1f, -0.2f, 0.2f);
        leftPhaseCoords[2] = new Vector3(-0.1f, -0.3f, 0.3f);
        rightPhaseCoords[2] = new Vector3(0.2f, -0.1f, 0.6f);
        phaseChecker = new PhaseChecker(leftPhaseCoords, rightPhaseCoords);
    }
    void Update()
    {
        if (StateManager.state == State.IDLE || StateManager.state == State.PATTERN1)
        {
            if (phaseChecker.check(0) || (StateManager.state == State.PATTERN1))
            {
                StateManager.state = State.PATTERN1;
                checkPattern();
            }
            StateManager.updateCountdown();
            updateHelper();
        }
        else
        {
            leftHelper.transform.localScale = new Vector3(0, 0, 0);
            rightHelper.transform.localScale = new Vector3(0, 0, 0);
        }
    }
    void checkPattern()
    {
        StateManager.switchPhase(0, 5f);
        if ((phaseChecker.check(1) && StateManager.currentPhase == 0) || (StateManager.state == State.PATTERN1 && StateManager.currentPhase > 0))
        {
            StateManager.switchPhase(1, 5f);
            if (phaseChecker.check(2) && StateManager.currentPhase == 1)
            {
                Debug.Log("success!");
                StateManager.resetState();
            }
        }
    }

    void updateHelper()
    {
        Vector3 scale = new Vector3(PhaseChecker.tolerance, PhaseChecker.tolerance, PhaseChecker.tolerance);
        if (leftHelper.transform.localScale != scale && rightHelper.transform.localScale != scale)
        {
            leftHelper.transform.localScale = new Vector3(PhaseChecker.tolerance, PhaseChecker.tolerance, PhaseChecker.tolerance);
            rightHelper.transform.localScale = new Vector3(PhaseChecker.tolerance, PhaseChecker.tolerance, PhaseChecker.tolerance);

        }

        Vector3 helperPosition = cameraTransform.position + offset;
        float angle = Vector3.Angle(helperPosition, cameraTransform.position);
        if (angle > 0.2f) helperPosition = Quaternion.AngleAxis(angle, Vector3.up) * helperPosition;

        leftHelper.transform.position = helperPosition + this.leftPhaseCoords[StateManager.currentPhase + 1];
        rightHelper.transform.position = helperPosition + this.rightPhaseCoords[StateManager.currentPhase + 1];

    }
}
