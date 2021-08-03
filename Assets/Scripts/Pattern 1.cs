using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern1 : MonoBehaviour
{
    PhaseChecker phaseChecker;
    Vector3[] leftPhaseCoords;
    Vector3[] rightPhaseCoords;

    void Start()
    {
        leftPhaseCoords = new Vector3[3];
        rightPhaseCoords = new Vector3[3];
        leftPhaseCoords[0]= new Vector3(1,1,1);
        leftPhaseCoords[1]= new Vector3(1,1,1);
        leftPhaseCoords[2]= new Vector3(1,1,1);
        rightPhaseCoords[0]= new Vector3(1,1,1);
        rightPhaseCoords[1]= new Vector3(1,1,1);
        rightPhaseCoords[2]= new Vector3(1,1,1);
        phaseChecker = new PhaseChecker(leftPhaseCoords, rightPhaseCoords);
    }
    void Update()
    {

    }
}
