using UnityEngine;

public class OrbMovement : MonoBehaviour
{
    [Header("Orb Setup")]
    private float speed = 2f;

    private float newSpeed;
    private float t;
    private float slowSpeed = 3.5f;
    private float mediumSpeed = 4.5f;
    private float fastSpeed = 8f;
    private float rotateSpeed = 200f;
    private int orbDamage = 20;
    private Rigidbody rb;

    [Header("Target Setup")]
    private Transform playerTarget;
    private Transform[] targets;
    public Transform target;
    public int targetIndex = 0;
    OrbManager orbManager;
    public GameObject enemyContainer;
    public bool isFinalEnemyTargetPassed = false;
    public bool isFinalPlayerTargetPassed = false;

    public bool hasTarget { get { return target != null; } }
    public bool targetIsPlayer { get { return target == playerTarget; } }

    void Start()
    {
        orbManager = OrbManager.instance;
        gameObject.name = "Orb" + orbManager.orbsCreated;
        orbManager.AddOrb(gameObject);

        rb = GetComponent<Rigidbody>();

        playerTarget = GameObject.Find("Main Camera").transform;
        enemyContainer = GameObject.Find("Enemy Container");
        SetTargetArrayToPlayer();
    }

    void FixedUpdate()
    {
        if (!hasTarget)
        {
            PlayerStats.life -= orbDamage;
            orbManager.RemoveOrb(gameObject);
            //TODO: add destroy orb particle effect
            Destroy(gameObject);
            return;
        }
        CheckSpeed();
        UpdateSpeed();
        TranslateOrb();
    }

    private void SetTargetArrayToPlayer()
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

    private void TranslateOrb()
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
        //check if target is PatternTarget
        if (targets[targetIndex].parent.gameObject.tag == "PatternTarget")
        {
            //check if pattern phase for next target has already been checked successfully
            if (targetIndex > StateManager.instance.currentPhase)
            {
                Debug.Log("phase does not match targetIndex");
                target = null;
                return;
            }
        }

        if (targets[targetIndex].parent.gameObject.tag == "Enemy")
        {
            if (targetIndex >= targets.Length - 1)
            {
                SetTargetArrayToPlayer();
                isFinalEnemyTargetPassed = true;
                return;
            }
        }

        if (targetIndex >= targets.Length - 1)
        {
            if (targets[targetIndex].parent.gameObject.tag == "PatternTarget")
            {
                Transform mostReachableEnemy = GetMostReachableEnemy();
                Transform[] enemyArray = new Transform[] { mostReachableEnemy };
                SetTargetArray(enemyArray);
                StartCoroutine(mostReachableEnemy.GetComponent<Enemy>().ReceiveOrb(this.gameObject));
                isFinalPlayerTargetPassed = true;
                return;
            }
            target = null;
            Debug.Log("last target reached!");
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
        }

        // case 2: target is first target in PatternTarget[]
        if ((target.parent.gameObject.tag == "PatternTarget" && targetIndex == 0 || target.parent.gameObject.tag == "Enemy" && targetIndex == 0) && newSpeed != fastSpeed)
        {
            newSpeed = fastSpeed;
            t = 0;
        }

        // case 3: target is second, or higher target in PatternTarget[]
        if ((target.parent.gameObject.tag == "PatternTarget" && targetIndex > 0 || target.parent.gameObject.tag == "Enemy" && targetIndex > 0) && newSpeed != slowSpeed)
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
    public Transform GetMostReachableEnemy()
    {
        float scalar = -Mathf.Infinity;
        Transform mostReachableEnemy = null;
        for (var i = 0; i < enemyContainer.transform.childCount; i++)
        {
            Vector3 enemyDistanceVector = enemyContainer.transform.GetChild(i).position - transform.position;
            if (scalar < Vector3.Dot(rb.velocity, enemyDistanceVector))
            {
                mostReachableEnemy = enemyContainer.transform.GetChild(i);
                scalar = Vector3.Dot(rb.velocity, enemyDistanceVector);
            }
        }
        return mostReachableEnemy;
    }
}
