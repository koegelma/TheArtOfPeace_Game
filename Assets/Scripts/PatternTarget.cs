using UnityEngine;

public class PatternTarget : MonoBehaviour
{
    private Transform playerTransform;
    private Transform enemyTransform;
    public Transform[] targets;
    //public Transform[] GetTargets { get { return targets; } }
    public bool isInitialized = false;

    private void Start()
    {
        if (gameObject.tag == "PatternTarget")
        {
            // TODO: change parenting when instantiatet in pattern script
            // change transform to parent transform
            playerTransform = GameObject.Find("Main Camera").transform;
            SetPosition(playerTransform, true);
            Vector3 angles = new Vector3(transform.rotation.x, playerTransform.rotation.eulerAngles.y, transform.rotation.z);
            transform.rotation = Quaternion.Euler(angles);
        }

        if (gameObject.tag == "Enemy")
        {
            enemyTransform = gameObject.transform.parent.transform;
            SetPosition(enemyTransform, false);
        }

        SetTargets();
        isInitialized = true;
    }

    private void Update()
    {
        if (gameObject.tag == "PatternTarget") SetPosition(playerTransform, true);
        if (gameObject.tag == "Enemy") SetPosition(enemyTransform, false);
    }

    private void SetPosition(Transform givenTransform, bool parentIsPlayer)
    {
        if (parentIsPlayer) transform.position = new Vector3(givenTransform.position.x, transform.position.y, givenTransform.position.z);
        else transform.position = new Vector3(givenTransform.position.x, givenTransform.position.y, givenTransform.position.z); //transform.y - givenTransform.y/2;
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
