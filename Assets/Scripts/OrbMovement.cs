using UnityEngine;
using System.Collections;

public class OrbMovement : MonoBehaviour
{
    [Header("Orb Setup")]
    private float speed = 2f;
    private float newSpeed;
    private float t;
    private float slowSpeed = 3f; // was 3.5
    private float mediumSpeed = 4f; // was 4.5
    private float fastSpeed = 6f; // was 8
    private float rotateSpeed = 200f;
    private int orbDamage = 0;
    private Rigidbody rb;
    public Difficulty tier;
    public bool targetIsController;
    [HideInInspector] public bool isMerged = false;

    [Header("Target Setup")]
    private Transform playerTarget;
    public Transform[] targets;
    public Transform target;
    public int targetIndex = 0;
    OrbManager orbManager;
    public GameObject enemyContainer;
    public bool isFinalEnemyTargetPassed = false;
    public bool isFinalPlayerTargetPassed = false;
    public bool hasTarget { get { return target != null; } }
    public bool targetIsPlayer { get { return target == playerTarget; } }
    private Controller rightController;

    void Start()
    {
        orbManager = OrbManager.instance;
        gameObject.name = "Orb" + orbManager.orbsCreated;
        orbManager.AddOrb(gameObject);

        rb = GetComponent<Rigidbody>();

        playerTarget = GameObject.Find("Main Camera").transform;
        enemyContainer = GameObject.Find("Enemy Container");
        rightController = PatternManager.instance.rightController;
        if (tier == Difficulty.EASY)
        {
            SetTargetArrayToPlayer(); //temporary fix - ideal would be to have this in Enemy imo
        }
    }

    void FixedUpdate()
    {
        if (!isMerged && (tier == Difficulty.MEDIUM || tier == Difficulty.HARD)) return;

        if (targetIsController)
        {
            FollowController();
            return;
        }

        if (!hasTarget) DestroyOrb();

        CheckSpeed();
        UpdateSpeed();
        TranslateOrbToTarget();
    }

    private void FollowController()
    {
        float distance = Vector3.Distance(rb.position, rightController.controllerPosition);

        if (distance > 1 && newSpeed != 8)
        {
            newSpeed = 8;
            t = 0;
        }
        if (distance < 1 && distance > 0.5 && newSpeed != 1)
        {
            newSpeed = 1;
            t = 0;
        }
        if (distance < 0.5 && distance > 0.25 && newSpeed != 0.5f)
        {
            newSpeed = 0.5f;
            t = 0;
        }
        if (distance < 0.25 && newSpeed != 0)
        {
            newSpeed = 0;
            t = 0;
        }
        UpdateSpeed();
        Translate();
    }

    private void Translate()
    {
        Vector3 direction = rightController.controllerPosition - rb.position;
        direction.Normalize();
        Vector3 rotateAmount = Vector3.Cross(direction, transform.forward);
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.forward * speed;
    }

    public void SetTargetArrayToPlayer()
    {
        Transform[] playerTargetArray = new Transform[] { playerTarget };
        SetTargetArray(playerTargetArray);
    }

    public void SetTargetArray(Transform[] _targets)
    {
        targets = _targets;
        targetIndex = 0;
        target = targets[targetIndex];
    }

    private void DestroyOrb()
    {
        PlayerStats.life -= orbDamage;
        orbManager.RemoveOrb(gameObject);
        //TODO: add destroy orb particle effect
        Destroy(gameObject);
    }

    private void TranslateOrbToTarget()
    {
        Vector3 direction = target.position - rb.position;
        direction.Normalize();
        Vector3 rotateAmount = Vector3.Cross(direction, transform.forward);
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.forward * speed;

        if (GetDistanceToTarget() <= 0.1f) GetNextTarget();
    }

    private void GetNextTarget()
    {
        //if (targets[targetIndex].parent.gameObject.GetComponent<PatternTarget>() && targets[targetIndex].parent.gameObject.GetComponent<PatternTarget>().isEnemyPattern)
        if (targetIndex >= targets.Length - 1)
        {
            if (targetIsPlayer)
            {
                GameManager.instance.PlayPlayerDamageSound();
                DestroyOrb();
            }
            SetTargetArrayToPlayer();
            isFinalEnemyTargetPassed = true;
            return;
        }
        targetIndex++;
        target = targets[targetIndex];
    }

    private void CheckSpeed()
    {
        // case 1: target is player/camera
        if (targetIsPlayer && newSpeed != mediumSpeed)
        {
            newSpeed = mediumSpeed;
            t = 0;
            return;
        }

        // case 2: target is first target in PatternTarget[]
        if (targetIndex == 0 && newSpeed != fastSpeed)
        {
            newSpeed = fastSpeed;
            t = 0;
        }

        // case 3: target is second, or higher target in PatternTarget[]
        if (targetIndex > 0 && newSpeed != slowSpeed)
        {
            newSpeed = slowSpeed;
            t = 0;
        }
    }

    private void UpdateSpeed()
    {
        speed = Mathf.Lerp(speed, newSpeed, t);
        t += 0.5f;
    }

    public float GetDistanceToTarget()
    {
        float distance = Vector3.Distance(rb.position, target.position);
        return distance;
    }

    public void SendOrbToEnemy(Vector3 controllerVelocityOverTime)
    {
        float maxAngle = Mathf.Infinity;
        Transform mostReachableEnemy = null;
        for (var i = 0; i < enemyContainer.transform.childCount; i++)
        {
            Vector3 enemyDirection = enemyContainer.transform.GetChild(i).position - rightController.controllerPosition;
            // add velocity vectors over time
            float enemyAngle = Vector3.Angle(controllerVelocityOverTime.normalized, enemyDirection);
            //Debug.Log("Enemy: " + enemyContainer.transform.GetChild(i).GetSiblingIndex() + ", Angle: " + enemyAngle);
            if (maxAngle > enemyAngle) // could implement something like  - && enemyAngle < x - to only assist aim if enemy is within x-degrees
            {
                mostReachableEnemy = enemyContainer.transform.GetChild(i);
                maxAngle = enemyAngle;
            }
        }
        Transform[] enemyArray = new Transform[] { mostReachableEnemy };
        SetTargetArray(enemyArray);
        StartCoroutine(mostReachableEnemy.GetComponent<Enemy>().ReceiveOrb(this.gameObject));
        //isFinalPlayerTargetPassed = true;
        targetIsController = false;
    }

    public IEnumerator PrepareDestroyingOrb()
    {
        yield return new WaitForSeconds(0.1f);
        DestroyOrb();
    }
}

