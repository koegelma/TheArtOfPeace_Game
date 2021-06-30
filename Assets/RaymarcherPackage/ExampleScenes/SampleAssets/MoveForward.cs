using UnityEngine;

/// <summary>
/// Simple forward motion
/// </summary>
public class MoveForward : MonoBehaviour
{
    public float moveSpeed = 0.4f;
    public float addRotationSpeed = 2;

    private void Update()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        transform.Rotate(0, 0, addRotationSpeed * Time.deltaTime, Space.Self);
    }
}
