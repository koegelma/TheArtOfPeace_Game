using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : MonoBehaviour
{
    [Header("Pattern individual variables")]
    // individual settings for each pattern - adjust settings in Inspector
    public State pattern;
    public Difficulty difficulty;
    public Vector3[] leftPhaseCoords;
    public Vector3[] rightPhaseCoords;
    //public GameObject patternTargetPrefab;
    //public float patternTargetsCountdownLength;
    public float countdownBetweenPhases = 5f;
    public float movementSpeed;
    public float tolerance;

    [Header("Scene variables")]
    // same references for every pattern - get scene references through PatternReference
    [HideInInspector] public GameObject leftHelper;
    [HideInInspector] public GameObject rightHelper;
    [HideInInspector] public GameObject leftChild;
    [HideInInspector] public GameObject rightChild;
    [HideInInspector] public Controller leftController;
    [HideInInspector] public Controller rightController;
    [HideInInspector] public Transform cameraTransform;
    public bool isSelected = false;
    [HideInInspector] public bool helperScaleIsZero = true;
    [HideInInspector] public List<GameObject> orbsDirectedAtPlayer;
    [HideInInspector] public List<GameObject> orbsDirectedAtController;
    private StateManager stateManager;
    private PhaseChecker phaseChecker;
    private OrbManager orbManager;
    private PatternReference patternReference;
    private GameObject targetsGameObject;
    private Transform[] targets;
    //private float patternTargetsCountdown;
    //private bool isCountdown;
    private bool isTriggerReady = true;
    private bool isInPattern = false;
    private int nextPhaseIndex = 0;
    public GameObject[] helperPrefabs;
    GameObject tempLeft = null;
    GameObject tempRight = null;
    private bool isMoving;
    [Header("Recording Pattern")]
    public bool isRecordingPattern = false;
    private Transform enemyContainer;
    private int phaseIndex = 0;
    private bool isRecording = false;
    private float nextRecordingTime = 0;
    private float recordingPeriod = 0.5f;
    private Vector3 controllerVelocityOverTime;
    private bool recordControllerVelocity = false;
    //private bool isPrimaryButtonReady = true;

    private void Awake()
    {
        leftPhaseCoords = GameData.CalcPlayerPhaseCoords(leftPhaseCoords);
        rightPhaseCoords = GameData.CalcPlayerPhaseCoords(rightPhaseCoords);
        phaseChecker = new PhaseChecker(leftPhaseCoords, rightPhaseCoords, tolerance);
    }

    private void Start()
    {
        PatternReference.instance.GetSceneReferences(this);
        orbManager = OrbManager.instance;
        stateManager = StateManager.instance;
        //patternTargetsCountdown = patternTargetsCountdownLength;
        //isCountdown = false;

        targetsGameObject = null;

        if (isRecordingPattern)
        {
            enemyContainer = GameObject.Find("Enemy Container").transform;
            foreach (Transform child in enemyContainer)
            {
                child.GetComponent<Enemy>().isUpdating = false;
            }
        }
    }
    private void Update()
    {
        if (!isSelected) return;

        if (isRecordingPattern)
        {
            RecordPosition();
            return;
        }

        if (recordControllerVelocity) RecordControllerVelocity();

        //if (isCountdown) HndDestroyCountdown();
        if (!orbManager.HasOrbs) return;
        if (targetsGameObject == null && stateManager.state == pattern && stateManager.currentPhase == leftPhaseCoords.Length - 1)
        {
            SetHelperScaleToZero();
            stateManager.resetState();
            isInPattern = false;
            nextPhaseIndex = 0;
        }

        if (stateManager.state == State.IDLE && isInPattern)
        {
            isInPattern = false;
            nextPhaseIndex = 0;
        }

        if (!isInPattern && stateManager.state == State.IDLE && orbManager.IsAnyOrbDirectedAtPlayer())
        {
            CheckForPattern();
            return;
        }

        if (isInPattern) CheckForPhase();
    }

    private void CheckForPattern()
    {
        UpdateFirstHelper(); // update behaviour for multiple patterns
        if (phaseChecker.FirstCheck(0))
        {
            // StartCoroutine(SpawnPatternTargets());

            orbsDirectedAtPlayer = orbManager.GetAllOrbsDirectedAtPlayer(); //
            foreach (GameObject orb in orbsDirectedAtPlayer)                // 
            {                                                               //
                orb.GetComponent<OrbMovement>().targetIsController = true;  //
            }                                                                  //
            stateManager.state = pattern;
            stateManager.switchPhase(0, countdownBetweenPhases);
            nextPhaseIndex = 1;
            isInPattern = true;
        }
    }

    private void CheckForPhase()
    {
        UpdateNextHelper();
        if (isMoving) return;
        if (stateManager.currentPhase < leftPhaseCoords.Length - 3)
        {
            if (phaseChecker.NextCheck(nextPhaseIndex))
            {
                stateManager.switchPhase(nextPhaseIndex, countdownBetweenPhases);
                nextPhaseIndex++;
            }
            return;
        }

        // second to last check: start recording velocity vectors of right controller
        if (stateManager.currentPhase < leftPhaseCoords.Length - 2)
        {
            if (phaseChecker.NextCheck(nextPhaseIndex))
            {
                stateManager.switchPhase(nextPhaseIndex, countdownBetweenPhases);
                nextPhaseIndex++;
                controllerVelocityOverTime = Vector3.zero;
                recordControllerVelocity = true;
            }
            return;
        }
        // final phase
        if (phaseChecker.NextCheck(nextPhaseIndex))
        {
            stateManager.switchPhase(nextPhaseIndex, countdownBetweenPhases);
            nextPhaseIndex++;
            stateManager.isFinalPhase = true;
            orbsDirectedAtController = orbManager.GetAllOrbsDirectedAtController();
            foreach (GameObject orb in orbsDirectedAtController)
            {
                //rightController.controllerVelocity
                //get enemy in controller direction (movement) and set target of orbsDirectedAtController to this enemy
                if (AssertDifficulty(orb)) orb.GetComponent<OrbMovement>().SendOrbToEnemy(controllerVelocityOverTime);
                else orb.GetComponent<OrbMovement>().PrepareDestroyingOrb();
            }
        }
    }

    private void RecordControllerVelocity()
    {
        if (stateManager.state == State.IDLE)
        {
            recordControllerVelocity = false;
            return;
        }

        controllerVelocityOverTime += rightController.controllerVelocity;
    }

    public void TogglePattern()
    {
        isSelected = !isSelected;
        if (isSelected) PatternManager.instance.activePattern = this;
        helperScaleIsZero = false;
    }

    private void UpdateFirstHelper()
    {
        if (stateManager.state == pattern && stateManager.currentPhase == leftPhaseCoords.Length - 1)
        {
            SetHelperScaleToZero();
            return;
        }
        leftHelper.transform.position = cameraTransform.position;
        rightHelper.transform.position = cameraTransform.position;

        leftChild.transform.localPosition = this.leftPhaseCoords[stateManager.currentPhase + 1];
        rightChild.transform.localPosition = this.rightPhaseCoords[stateManager.currentPhase + 1];

        leftHelper.transform.eulerAngles = new Vector3(leftHelper.transform.eulerAngles.x, cameraTransform.eulerAngles.y, leftHelper.transform.eulerAngles.z);
        rightHelper.transform.eulerAngles = new Vector3(rightHelper.transform.eulerAngles.x, cameraTransform.eulerAngles.y, rightHelper.transform.eulerAngles.z);

        phaseChecker.globalLeftPhaseCoord = leftChild.transform.position;
        phaseChecker.globalRightPhaseCoord = rightChild.transform.position;


        float helperTolerance = tolerance * 2;
        Vector3 newScale = new Vector3(helperTolerance, helperTolerance, helperTolerance);
        if (leftChild.transform.localScale != newScale && rightChild.transform.localScale != newScale)
        {
            leftChild.transform.localScale = new Vector3(helperTolerance, helperTolerance, helperTolerance);
            rightChild.transform.localScale = new Vector3(helperTolerance, helperTolerance, helperTolerance);
            helperScaleIsZero = false;
        }
    }

    private void UpdateNextHelper()
    {
        if (stateManager.state == pattern && stateManager.currentPhase == leftPhaseCoords.Length - 1)
        {
            SetHelperScaleToZero();
            return;
        }
        leftHelper.transform.position = cameraTransform.position;
        rightHelper.transform.position = cameraTransform.position;

        MoveChildToTarget(leftChild.transform, this.leftPhaseCoords[stateManager.currentPhase + 1]);
        MoveChildToTarget(rightChild.transform, this.rightPhaseCoords[stateManager.currentPhase + 1]);
    }

    private void MoveChildToTarget(Transform childTransform, Vector3 phaseCoordPosition)
    {
        if (Vector3.Distance(childTransform.localPosition, phaseCoordPosition) > 0.05f)
        {
            isMoving = true;
            childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, phaseCoordPosition, Time.deltaTime * movementSpeed);
            return;
        }
        childTransform.localPosition = phaseCoordPosition;
        phaseChecker.globalLeftPhaseCoord = leftChild.transform.position;
        phaseChecker.globalRightPhaseCoord = rightChild.transform.position;
        isMoving = false;
    }

    private void SetHelperScaleToZero()
    {
        leftChild.transform.localScale = Vector3.zero;
        rightChild.transform.localScale = Vector3.zero;
        helperScaleIsZero = true;
    }

    private bool AssertDifficulty(GameObject orb)
    {
        Difficulty orbTier = orb.GetComponent<OrbMovement>().tier;
        switch (difficulty)
        {
            case Difficulty.EASY:
                if (orbTier == Difficulty.EASY) return true;
                break;
            case Difficulty.MEDIUM:
                if (orbTier == Difficulty.EASY || orbTier == Difficulty.MEDIUM) return true;
                break;
            case Difficulty.HARD:
                if (orbTier == Difficulty.EASY || orbTier == Difficulty.MEDIUM || orbTier == Difficulty.HARD) return true;
                break;
        }
        Debug.Log("Difficulty does not match");
        return false;
    }

    private void RecordPosition()
    {
        if (leftController.isTrigger && rightController.isTrigger && isTriggerReady)
        {
            isTriggerReady = false;
            isRecording = !isRecording;
            if (isRecording)
            {
                nextRecordingTime = Time.time + recordingPeriod;
                AddNewPhaseCoord(true);
                //Debug.Log("left: " + leftController.relativeTransform.position);
                tempLeft = (GameObject)Instantiate(helperPrefabs[0], leftController.controllerPosition, Quaternion.identity);
                //Debug.Log("right: " + rightController.relativeTransform.position);
                tempRight = (GameObject)Instantiate(helperPrefabs[1], rightController.controllerPosition, Quaternion.identity);
                return;
            }
        }
        if (!isTriggerReady && !leftController.isTrigger && !rightController.isTrigger) isTriggerReady = true;
        if (!isRecording) return;

        if (Time.time > nextRecordingTime)
        {
            nextRecordingTime += recordingPeriod;
            AddNewPhaseCoord(false);
            tempLeft = (GameObject)Instantiate(helperPrefabs[0], leftController.controllerPosition, Quaternion.identity);
            tempRight = (GameObject)Instantiate(helperPrefabs[1], rightController.controllerPosition, Quaternion.identity);
        }
    }

    private void AddNewPhaseCoord(bool isFirstCoord)
    {
        if (isFirstCoord)
        {
            leftPhaseCoords[phaseIndex] = leftController.relativeTransform.position;
            rightPhaseCoords[phaseIndex] = rightController.relativeTransform.position;
        }
        else
        {
            leftPhaseCoords[phaseIndex] = leftController.nextRelativeTransform.position;
            rightPhaseCoords[phaseIndex] = rightController.nextRelativeTransform.position;
        }
        phaseIndex++;
    }
}