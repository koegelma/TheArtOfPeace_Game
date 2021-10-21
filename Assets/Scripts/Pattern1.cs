﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern1 : MonoBehaviour
{
    public PhaseChecker phaseChecker;
    public Vector3[] leftPhaseCoords;
    public Vector3[] rightPhaseCoords;
    public GameObject leftHelper;
    public GameObject rightHelper;
    public GameObject leftChild;
    public GameObject rightChild;
    public Controller leftController;
    public Controller rightController;
    public Transform cameraTransform;
    public GameObject patternTargetPrefab;
    private GameObject targetsGameObject;
    private Transform[] targets;
    private OrbManager orbManager;
    private float patternTargetsCountdown;
    private float patternTargetsCountdownLength = 7f;
    private bool isCountdown = false;
    //private Vector3 offset = new Vector3(0, 0, 2);

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

        targetsGameObject = null;
    }

    private void Start()
    {
        orbManager = OrbManager.instance;
        patternTargetsCountdown = patternTargetsCountdownLength;
    }
    void Update()
    {
        if (isCountdown) Countdown();
        if (!orbManager.HasOrbs) return;

        //TODO: updateHelper soll nicht in IDLE aufgerufen werden
        // - target array for OrbMovement: only set target[n] if phase(n) is switched?
        if (StateManager.state == State.IDLE && orbManager.IsAnyOrbDirectedAtPlayer() || StateManager.state == State.PATTERN1)
        {
            if (phaseChecker.check(0) || (StateManager.state == State.PATTERN1))
            {
                if (StateManager.state != State.PATTERN1 && targetsGameObject == null) StartCoroutine(SpawnPatternTargets());
                StateManager.state = State.PATTERN1;
                checkPattern();
            }
            StateManager.updateCountdown();
            updateHelper();
        }
        else
        {
            leftChild.transform.localScale = new Vector3(0, 0, 0);
            rightChild.transform.localScale = new Vector3(0, 0, 0);
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
        float tolerance = PhaseChecker.tolerance * 2;
        Vector3 scale = new Vector3(tolerance, tolerance, tolerance);
        if (leftChild.transform.localScale != scale && rightChild.transform.localScale != scale)
        {
            leftChild.transform.localScale = new Vector3(tolerance, tolerance, tolerance);
            rightChild.transform.localScale = new Vector3(tolerance, tolerance, tolerance);
        }

        leftHelper.transform.position = cameraTransform.position;
        rightHelper.transform.position = cameraTransform.position;

        leftChild.transform.localPosition = this.leftPhaseCoords[StateManager.currentPhase + 1];
        rightChild.transform.localPosition = this.rightPhaseCoords[StateManager.currentPhase + 1];

        leftHelper.transform.eulerAngles = new Vector3(leftHelper.transform.eulerAngles.x, cameraTransform.eulerAngles.y, leftHelper.transform.eulerAngles.z);
        rightHelper.transform.eulerAngles = new Vector3(rightHelper.transform.eulerAngles.x, cameraTransform.eulerAngles.y, rightHelper.transform.eulerAngles.z);
    }

    private IEnumerator SpawnPatternTargets()
    {
        targetsGameObject = (GameObject)Instantiate(patternTargetPrefab, Vector3.zero, transform.rotation);
        isCountdown = true;
        PatternTarget targetsScript = targetsGameObject.GetComponent<PatternTarget>();
        yield return new WaitUntil(() => targetsScript.isInitialized);

        targets = targetsScript.targets;

        List<GameObject> orbsDirectedAtPlayer = orbManager.GetAllOrbsDirectedAtPlayer();

        foreach (GameObject orb in orbsDirectedAtPlayer)
        {
            orb.GetComponent<OrbMovement>().SetTargetArray(targets);
        }
    }

    private void Countdown()
    {
        if (patternTargetsCountdown <= 0f)
        {
            DestroyPatternTargets();
            patternTargetsCountdown = patternTargetsCountdownLength;
            isCountdown = false;
            return;
        }
        patternTargetsCountdown -= Time.deltaTime;
    }

    private void DestroyPatternTargets()
    {
        Destroy(targetsGameObject);
        targetsGameObject = null;
    }
}