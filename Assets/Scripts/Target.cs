using UnityEngine;

public class Target : MonoBehaviour
{
    private Transform playerTransform;
    public Transform[] targets;
    //public Transform[] GetTargets { get { return targets; } }
    public bool IsInitialized = false;

    private void Start()
    {
        playerTransform = GameObject.Find("Main Camera").transform;
        SetPosition();
        Vector3 angles = new Vector3(transform.rotation.x, playerTransform.rotation.y*100, transform.rotation.z);
        transform.rotation = Quaternion.Euler(angles);

        SetTargets();
        IsInitialized = true;
    }

    private void Update()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        transform.position = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
    }

    private void SetTargets()
    {
        targets = new Transform[transform.childCount];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = transform.GetChild(i);
        }
    }
}
