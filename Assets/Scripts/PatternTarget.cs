using UnityEngine;

public class PatternTarget : MonoBehaviour
{
    private Transform playerTransform;
    private Transform enemyTransform;
    public Transform[] targets;
    //public Transform[] GetTargets { get { return targets; } }
    public bool isInitialized = false;
    public bool isEnemyPattern;
    public Difficulty difficulty;

    private void Start()
    {
        if (isEnemyPattern)
        {
            enemyTransform = gameObject.transform.parent.transform;
            SetPosition(enemyTransform);
        }
        SetTargets();
        isInitialized = true;
    }

    private void Update()
    {
        if (isEnemyPattern) SetPosition(enemyTransform);
    }

    private void SetPosition(Transform givenTransform)
    {
        transform.position = new Vector3(givenTransform.position.x, givenTransform.position.y + 1, givenTransform.position.z); //transform.y - givenTransform.y/2;
        //else transform.position = Vector3.zero;
    }

    private void SetTargets()
    {
        targets = new Transform[transform.childCount];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = transform.GetChild(i);
        }
    }

    public void SetEnemyTransform(Transform _enemyTransform)
    {
        enemyTransform = _enemyTransform;
    }
}
