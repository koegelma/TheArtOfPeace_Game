using UnityEngine;

/// <summary>
/// Simple object movement
/// </summary>
public class FollowMovement : MonoBehaviour
{
    private Camera cameraCache;

    public Transform[] bodyRefs;
    public float followDist = 1.0f;
    public float depth = 4;

    private void Start()
    {
        cameraCache = Camera.main;    
    }

    private void Update()
    {
        if(bodyRefs.Length > 1)
            for (int i = 1; i < bodyRefs.Length; i++)
            {
                float actualDistance = Vector3.Distance(bodyRefs[i].position, bodyRefs[i - 1].position);
                if (actualDistance > followDist)
                {
                    Vector3 followToCurrent = (bodyRefs[i].position - bodyRefs[i - 1].position).normalized;
                    followToCurrent.Scale(Vector3.one * followDist);
                    bodyRefs[i].position = Vector3.Lerp(bodyRefs[i].position, bodyRefs[i - 1].position + followToCurrent, 16 * Time.deltaTime);
                }
            }

        if (!Input.GetMouseButton(0)) return;

        Vector3 finger = Input.mousePosition;
        Vector3 p = cameraCache.ScreenToWorldPoint(new Vector3(finger.x, finger.y, depth));
        bodyRefs[0].position = Vector3.Lerp(bodyRefs[0].position, p, 16 * Time.deltaTime);
        bodyRefs[0].rotation = Quaternion.LookRotation(bodyRefs[0].position - cameraCache.transform.position);
    }
}
