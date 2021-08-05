using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern1 : MonoBehaviour
{
    PhaseChecker phaseChecker;
    Vector3[] leftPhaseCoords;
    Vector3[] rightPhaseCoords;
    /* public Controller leftController;
    public Controller rightController; */
    
    void Start()
    {
        leftPhaseCoords = new Vector3[3];
        rightPhaseCoords = new Vector3[3];
        leftPhaseCoords[0] = new Vector3(-0.2f, -0.1f, 0.5f);
        rightPhaseCoords[0] = new Vector3(0.1f, -0.3f, 0.1f);
        leftPhaseCoords[1] = new Vector3(-0.1f, -0.2f, 0.2f);
        rightPhaseCoords[1] = new Vector3(0.1f, -0.2f, 0.2f);
        leftPhaseCoords[2] = new Vector3(-0.1f, -0.3f, 0.1f);
        rightPhaseCoords[2] = new Vector3(0.2f, -0.1f, 0.5f);
        phaseChecker = new PhaseChecker(leftPhaseCoords, rightPhaseCoords);
    }
    void Update()
    {
        /* if (leftController.isTrigger && rightController.isTrigger){
            Debug.Log("left: "+leftController.relativeTransform.position);
            Debug.Log("right: "+rightController.relativeTransform.position);
        } */
        
        if(StateManager.state == State.IDLE || StateManager.state == State.PATTERN1){
            if (phaseChecker.check(0) || (StateManager.state == State.PATTERN1 && StateManager.currentPhase >= 0))
            {
                Debug.Log("phase0");
                StateManager.state = State.PATTERN1;
                checkPattern();
                StateManager.updateStates();
            }
        }
    }
    void checkPattern()
    {
        StateManager.switchPhase(0, 5f);
        if ((phaseChecker.check(1) && StateManager.currentPhase == 0) || (StateManager.state == State.PATTERN1 && StateManager.currentPhase > 0))
        {
            Debug.Log("phase1");
            StateManager.switchPhase(1, 5f);
            if (phaseChecker.check(2) && StateManager.currentPhase == 1)
            {
                Debug.Log("success!");
                StateManager.resetState();
            }
        }
    }

    
}
