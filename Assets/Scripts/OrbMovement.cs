using UnityEngine;

public class OrbMovement : MonoBehaviour
{
    [Header("Orb Setup")]
    public float speed = 2f;
    public float rotateSpeed = 200f;
    private Rigidbody rb;

    [Header("Target Setup")]
    private Transform playerTarget;
    private Transform[] targets;
    public Transform target;
    private int targetIndex = 0;
    OrbManager orbManager;
    //GameObject patternNode;//remove - only for testing
    public GameObject enemyContainer;

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
            orbManager.RemoveOrb(gameObject);
            //TODO: add destroy orb particle effect
            Destroy(gameObject);
            return;
        }
        TranslateOrb();
    }

    private void SetTargetArrayToPlayer()
    {
        // TODO: check if State != IDLE or if pattern exists, then set target[] to pattern
        Transform[] playerTargetArray = new Transform[] { playerTarget };
        SetTargetArray(playerTargetArray);
    }
    /* public void SetPatternNode(GameObject pattern) //remove - only for testing
    {
        patternNode = pattern;
    } */

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
        Vector3 rotateAmount = Vector3.Cross(direction, transform.up); // fix rotation
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.up * speed;

        if (GetDistanceToTarget() <= 0.1f) GetNextTarget();
    }

    private void GetNextTarget()
    {
        //check if target is PatternTarget
        if (targets[targetIndex].parent.gameObject.tag == "PatternTarget")
        {
            //check if pattern phase for next target has already been checked successfully
            if (targetIndex > StateManager.currentPhase)
            {
                Debug.Log("phase does not match targetIndex");
                target = null;
                return;
            }
        }

        if (targetIndex >= targets.Length - 1)
        {
            if (target.name.Contains("Node"))
            {
                Transform[] enemyArray = new Transform[] { GetMostReachableEnemy() };
                SetTargetArray(enemyArray);
                // Destroy targets (GameObject Targets) from here
                //Destroy(patternNode);//remove - only for testing
                return;
            }
            target = null;
            Debug.Log("last target reached!");
            return;
        }
        targetIndex++;
        target = targets[targetIndex];
    }

    /*  private void HndPlayerTargeting()
     {
         //StateManager.state != State.IDLE
         // if (transform.parent.gameObject.name.Equals("Left Controller")) this.controller = GameObject.Find("Left Controller").GetComponent<Controller>();
         // if (transform.parent.gameObject.tag == "PatternTarget"
     } */

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
