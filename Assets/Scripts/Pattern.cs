using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : MonoBehaviour
{
    [Header("Pattern individual variables")]
    // individual settings for each pattern - adjust settings in Inspector
    public State pattern;
    public Vector3[] leftPhaseCoords;
    public Vector3[] rightPhaseCoords;
    public GameObject patternTargetPrefab;
    public float patternTargetsCountdownLength;
    public float countdownBetweenPhases = 5f;
    public float movementSpeed;
    public bool isTestPattern;
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
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public bool helperScaleIsZero = true;
    [HideInInspector] public List<GameObject> orbsDirectedAtPlayer;
    private StateManager stateManager;
    private PhaseChecker phaseChecker;
    private OrbManager orbManager;
    private PatternReference patternReference;
    private GameObject targetsGameObject;
    private Transform[] targets;
    private float patternTargetsCountdown;
    private bool isCountdown;
    private bool isTriggerReady = true;
    private bool isInPattern = false;
    private int nextPhaseIndex = 0;
    public GameObject[] helperPrefabs;
    GameObject tempLeft = null;
    GameObject tempRight = null;
    private bool isMoving;
    private int phaseIndex = 0;
    private bool isPrimaryButtonReady = true;


    private void TestPosition()
    {
        if (leftController.isTrigger && rightController.isTrigger && isTriggerReady)
        {
            isTriggerReady = false;
            if (phaseIndex > 0) AddNewPhaseCoord(false);
            else AddNewPhaseCoord(true);
            //Debug.Log("left: " + leftController.relativeTransform.position);
            tempLeft = (GameObject)Instantiate(helperPrefabs[0], leftController.controllerPosition, Quaternion.identity);
            //Debug.Log("right: " + rightController.relativeTransform.position);
            tempRight = (GameObject)Instantiate(helperPrefabs[1], rightController.controllerPosition, Quaternion.identity);

            Vector3 patternTargetPos = (leftController.controllerPosition + rightController.controllerPosition) / 2;
            Debug.Log(phaseIndex - 1 + ": " + patternTargetPos);
        }
        if (!isTriggerReady && !leftController.isTrigger && !rightController.isTrigger) isTriggerReady = true;
        if (rightController.isPrimaryButton && isPrimaryButtonReady && phaseIndex > 0)
        {
            isPrimaryButtonReady = false;
            Destroy(tempLeft);
            Destroy(tempRight);
            leftPhaseCoords[phaseIndex] = Vector3.zero;
            rightPhaseCoords[phaseIndex] = Vector3.zero;
            phaseIndex--;

            Debug.Log("Destroyed");
        }
        if (!rightController.isPrimaryButton && !isPrimaryButtonReady) isPrimaryButtonReady = true;
    }

    private void AddNewPhaseCoord(bool isFirst)
    {
        if (isFirst)
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
        patternTargetsCountdown = patternTargetsCountdownLength;
        isCountdown = false;
        targetsGameObject = null;
    }
    private void Update()
    {
        if (!isSelected) return;
        if (isTestPattern)
        {
            TestPosition();
            return;
        }
        //TestPosition();
        //return;
        if (isCountdown) HndDestroyCountdown();
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
            StartCoroutine(SpawnPatternTargets());
            stateManager.state = pattern;
            stateManager.switchPhase(0, countdownBetweenPhases);
            nextPhaseIndex = 1;
            isInPattern = true;
        }
    }

    private void CheckForPhase()
    {
        UpdateNextHelper();
        // check if not final phase
        if (isMoving) return;
        if (stateManager.currentPhase < leftPhaseCoords.Length - 1)
        {
            if (phaseChecker.NextCheck(nextPhaseIndex))
            {
                stateManager.switchPhase(nextPhaseIndex, countdownBetweenPhases);
                nextPhaseIndex++;
            }
            return;
        }
        // final phase
        stateManager.isFinalPhase = true;
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

    private IEnumerator SpawnPatternTargets()
    {
        targetsGameObject = (GameObject)Instantiate(patternTargetPrefab, Vector3.zero, transform.rotation);
        PatternTarget targetsScript = targetsGameObject.GetComponent<PatternTarget>();
        yield return new WaitUntil(() => targetsScript.isInitialized);

        targets = targetsScript.targets;

        orbsDirectedAtPlayer = orbManager.GetAllOrbsDirectedAtPlayer();
        isCountdown = true;
        foreach (GameObject orb in orbsDirectedAtPlayer)
        {
            orb.GetComponent<OrbMovement>().SetTargetArray(targets);
        }
    }

    private bool CheckPatternTargetStatus()
    {
        //check individually if orb==null then remove from this.orbsDirectedAtPlayer

        //check if all orbs have passed last target in array
        bool allPassed = true;
        foreach (GameObject orb in orbsDirectedAtPlayer)
        {
            if (orb != null && !orb.GetComponent<OrbMovement>().isFinalPlayerTargetPassed)
            {
                allPassed = false;
                break;
            }
        }
        //check if all orbs == null
        bool allNull = true;
        foreach (GameObject orb in orbsDirectedAtPlayer)
        {
            if (orb != null)
            {
                allNull = false;
                break;
            }
        }
        if (allPassed || allNull)
        {
            if (allPassed)
            {
                foreach (GameObject orb in orbsDirectedAtPlayer)
                {
                    if (orb != null) orb.GetComponent<OrbMovement>().isFinalPlayerTargetPassed = false;
                }
            }
            DestroyPatternTargets();
            return true;
        }
        return false;
    }

    private void HndDestroyCountdown()
    {
        if (CheckPatternTargetStatus()) return;
        if (patternTargetsCountdown <= 0f)
        {
            DestroyPatternTargets();
            return;
        }
        patternTargetsCountdown -= Time.deltaTime;
    }

    private void DestroyPatternTargets()
    {
        SetHelperScaleToZero();
        patternTargetsCountdown = patternTargetsCountdownLength;
        isCountdown = false;
        Destroy(targetsGameObject);
        targetsGameObject = null;

        stateManager.resetState();
        isInPattern = false;
        nextPhaseIndex = 0;
    }
}