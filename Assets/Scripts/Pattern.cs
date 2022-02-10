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
    private Transform[] targets;
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
    private float recordingPeriod = 0.25f;
    private Vector3 controllerVelocityOverTime;
    private bool recordControllerVelocity = false;
    [HideInInspector] public GameObject secondNextRightPhaseCoord;
    [HideInInspector] public GameObject secondNextLeftPhaseCoord;
    public GameObject recordedPatternContainerLeft;
    public GameObject recordedPatternContainerRight;
    private int recordedPhases = 0;

    private void Awake()
    {
        leftPhaseCoords = GameData.CalcPlayerPhaseCoords(leftPhaseCoords);
        rightPhaseCoords = GameData.CalcPlayerPhaseCoords(rightPhaseCoords);
        phaseChecker = new PhaseChecker(leftPhaseCoords, rightPhaseCoords, tolerance, pattern);
    }

    private void Start()
    {
        PatternReference.instance.GetSceneReferences(this);
        orbManager = OrbManager.instance;
        stateManager = StateManager.instance;

        secondNextRightPhaseCoord.transform.parent = leftHelper.transform;
        secondNextLeftPhaseCoord.transform.parent = leftHelper.transform;

        /* if (isRecordingPattern)
        {
            enemyContainer = GameObject.Find("Enemy Container").transform;
            foreach (Transform child in enemyContainer)
            {
                child.GetComponent<Enemy>().isUpdating = false;
            }
        } */
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
        if (stateManager.state == pattern && stateManager.currentPhase == leftPhaseCoords.Length - 1)
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
        if (!isTriggerReady && !leftController.isTrigger && !rightController.isTrigger) isTriggerReady = true;
        UpdateFirstHelper();
        if (phaseChecker.CheckForRightPosition(0) && !rightController.isSendingHapticFeedback && !leftController.isSendingHapticFeedback)
        {
            leftController.HapticImpulse(0.1f, 0.05f);
            rightController.HapticImpulse(0.1f, 0.05f);
        }
        if (phaseChecker.FirstCheck(0) && isTriggerReady)
        {
            orbsDirectedAtPlayer = orbManager.GetAllOrbsDirectedAtPlayer();
            foreach (GameObject orb in orbsDirectedAtPlayer)
            {
                orb.GetComponent<OrbMovement>().targetIsController = true;
            }
            stateManager.state = pattern;
            stateManager.switchPhase(0, countdownBetweenPhases);
            nextPhaseIndex = 1;
            isInPattern = true;
            isTriggerReady = false;
        }
    }

    private void CheckForPhase()
    {
        if (!leftController.isTrigger && !rightController.isTrigger) CancelPattern();
        UpdateNextHelper();
        if (isMoving) return;

        ///////DELETE ME

        if ((pattern == State.WAVES && (stateManager.currentPhase == 7 || stateManager.currentPhase == 17)) || (pattern == State.TURN && (stateManager.currentPhase == 6)))
        {
            if (phaseChecker.NextCheck(nextPhaseIndex))
            {
                StartCoroutine(WaitForPhaseSwitch());
                isSelected = false;
                return;
            }
        }


        /////////////

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
                if (AssertDifficulty(orb)) orb.GetComponent<OrbMovement>().SendOrbToEnemy(controllerVelocityOverTime);
                else StartCoroutine(orb.GetComponent<OrbMovement>().PrepareDestroyingOrb());
            }
        }
    }

    private IEnumerator WaitForPhaseSwitch()
    {
        yield return new WaitForSeconds(0.25f);
        stateManager.switchPhase(nextPhaseIndex, countdownBetweenPhases);
        nextPhaseIndex++;
        isSelected = true;
    }

    private void CancelPattern()
    {
        SetHelperScaleToZero();
        stateManager.resetState();
        isInPattern = false;
        nextPhaseIndex = 0;

        leftController.HapticImpulse(0.4f, 0.5f);
        rightController.HapticImpulse(0.4f, 0.5f);
        GameManager.instance.PlayNegativeUISound();
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

        if (!rightChild.transform.GetChild(0).gameObject.activeSelf && !leftChild.transform.GetChild(0).gameObject.activeSelf)
        {
            rightChild.transform.GetChild(0).gameObject.SetActive(true);
            leftChild.transform.GetChild(0).gameObject.SetActive(true);
        }

        leftHelper.transform.position = cameraTransform.position;
        rightHelper.transform.position = cameraTransform.position;

        leftChild.transform.localPosition = this.leftPhaseCoords[stateManager.currentPhase + 1];
        rightChild.transform.localPosition = this.rightPhaseCoords[stateManager.currentPhase + 1];

        leftHelper.transform.eulerAngles = new Vector3(leftHelper.transform.eulerAngles.x, cameraTransform.eulerAngles.y, leftHelper.transform.eulerAngles.z);
        rightHelper.transform.eulerAngles = new Vector3(rightHelper.transform.eulerAngles.x, cameraTransform.eulerAngles.y, rightHelper.transform.eulerAngles.z);

        phaseChecker.globalLeftPhaseCoord = leftChild.transform.position;
        phaseChecker.globalRightPhaseCoord = rightChild.transform.position;

        UpdateArrow();

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

        if (stateManager.currentPhase < leftPhaseCoords.Length - 2) UpdateArrow();
        else
        {
            rightChild.transform.GetChild(0).gameObject.SetActive(false);
            leftChild.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void UpdateArrow()
    {
        secondNextRightPhaseCoord.transform.localPosition = rightPhaseCoords[stateManager.currentPhase + 2];
        secondNextLeftPhaseCoord.transform.localPosition = leftPhaseCoords[stateManager.currentPhase + 2];
        rightChild.transform.LookAt(secondNextRightPhaseCoord.transform);
        leftChild.transform.LookAt(secondNextLeftPhaseCoord.transform);
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
        if (!isTriggerReady && !leftController.isTrigger && !rightController.isTrigger) isTriggerReady = true;
        if (leftController.isTrigger && rightController.isTrigger && isTriggerReady)
        {
            isTriggerReady = false;
            isRecording = !isRecording;
            if (isRecording)
            {
                nextRecordingTime = Time.time + recordingPeriod;
                //Debug.Log("left: " + leftController.relativeTransform.position);
                tempLeft = (GameObject)Instantiate(helperPrefabs[0], leftController.controllerPosition, Quaternion.identity);
                tempLeft.transform.parent = recordedPatternContainerLeft.transform;
                //Debug.Log("right: " + rightController.relativeTransform.position);
                tempRight = (GameObject)Instantiate(helperPrefabs[1], rightController.controllerPosition, Quaternion.identity);
                tempRight.transform.parent = recordedPatternContainerRight.transform;
                recordedPhases++;
                return;
            }
            Destroy(recordedPatternContainerLeft.transform.GetChild(0).GetChild(0).gameObject);
            Destroy(recordedPatternContainerLeft.transform.GetChild(0).GetChild(0).gameObject);
            Destroy(recordedPatternContainerRight.transform.GetChild(0).GetChild(0).gameObject);
            Destroy(recordedPatternContainerRight.transform.GetChild(0).GetChild(0).gameObject);
            // Stop Recording: Rotate and shit

            /* recordedPatternContainerLeft.transform.GetChild(0).position = Vector3.zero;
            recordedPatternContainerRight.transform.GetChild(0).position = Vector3.zero;
            recordedPatternContainerLeft.transform.LookAt(Vector3.forward);
            recordedPatternContainerRight.transform.LookAt(Vector3.forward);

            float rightLength = Vector3.Distance()

            recordedPatternContainerLeft.transform.GetChild(1).position = Vector3.zero;
            recordedPatternContainerRight.transform.GetChild(1).position = Vector3.zero; */

        }
        if (!isRecording) return;

        if (Time.time > nextRecordingTime)
        {
            nextRecordingTime += recordingPeriod;
            tempLeft = (GameObject)Instantiate(helperPrefabs[0], leftController.controllerPosition, Quaternion.identity);
            //tempLeft.transform.parent = recordedPatternContainerLeft.transform;
            tempRight = (GameObject)Instantiate(helperPrefabs[1], rightController.controllerPosition, Quaternion.identity);
            //tempRight.transform.parent = recordedPatternContainerRight.transform;
            recordedPhases++;

            if (recordedPhases > 4)
            {
                tempLeft.transform.parent = recordedPatternContainerLeft.transform.GetChild(0).GetChild(1);
                tempRight.transform.parent = recordedPatternContainerLeft.transform.GetChild(0).GetChild(1);
                return;
            }
            tempLeft.transform.parent = recordedPatternContainerLeft.transform.GetChild(0);
            tempRight.transform.parent = recordedPatternContainerRight.transform.GetChild(0);
        }
    }
}