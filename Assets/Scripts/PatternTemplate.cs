using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternTemplate : MonoBehaviour
{
    PhaseChecker phaseChecker;
    Vector3[] leftPhaseCoords;
    Vector3[] rightPhaseCoords;
    
    void Start()
    {
        leftPhaseCoords = new Vector3[3];
        rightPhaseCoords = new Vector3[3];
        leftPhaseCoords[0] = new Vector3(1, 1, 1);
        leftPhaseCoords[1] = new Vector3(1, 1, 1);
        leftPhaseCoords[2] = new Vector3(1, 1, 1);
        rightPhaseCoords[0] = new Vector3(1, 1, 1);
        rightPhaseCoords[1] = new Vector3(1, 1, 1);
        rightPhaseCoords[2] = new Vector3(1, 1, 1);
        phaseChecker = new PhaseChecker(leftPhaseCoords, rightPhaseCoords);
    }
    void Update()
    {
        if(StateManager.state == State.IDLE || StateManager.state == State.PATTERN1){
            if (phaseChecker.check(0) || (StateManager.state == State.PATTERN1 && StateManager.currentPhase >= 0))
            {
                StateManager.state = State.PATTERN1;
                checkPattern();
            }
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
                StateManager.resetState();
            }
        }
    }

    
}
