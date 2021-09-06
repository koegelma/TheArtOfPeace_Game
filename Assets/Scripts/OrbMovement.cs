using System.Collections;
using System.Collections.Generic;
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
    private Transform target;
    private int targetIndex = 0;

    OrbManager orbManager;

    public bool HasTarget { get { return target != null; } }
    public bool TargetIsPlayer { get { return target == playerTarget; } }

    void Start()
    {
        orbManager = OrbManager.instance;
        gameObject.name = "Orb" + orbManager.OrbsInGame;
        orbManager.AddOrb(gameObject);

        rb = GetComponent<Rigidbody>();

        playerTarget = GameObject.Find("Main Camera").transform;
        SetTargetArrayToPlayer();
    }

    void FixedUpdate()
    {
        if (!HasTarget)
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
        Vector3 rotateAmount = Vector3.Cross(direction, transform.up);
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.up * speed;

        if (GetDistanceToTarget() <= 0.1f) GetNextTarget();
    }

    private void GetNextTarget()
    {
        if (targetIndex >= targets.Length - 1)
        {
            target = null;
            Debug.Log("last target reached!");
            return;
        }
        targetIndex++;
        target = targets[targetIndex];
    }

    public float GetDistanceToTarget()
    {
        float distance = Vector3.Distance(rb.position, target.position);
        return distance;
    }
}
