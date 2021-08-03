using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public GameObject target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    public Controller left;
    public Controller right;

    Vector3 offset;
    void OnEnable()
    {
        left = GameObject.Find("LeftController").GetComponent<Controller>();
        right = GameObject.Find("RightController").GetComponent<Controller>();
    }
    public void control()
    {
        float distanceLeft = (left.gameObject.transform.position - this.transform.position).magnitude;
        float distanceRight = (right.gameObject.transform.position - this.transform.position).magnitude;
        if (distanceLeft < distanceRight)
        {
            target = left.gameObject;
        }
        else
        {
            target = right.gameObject;
        }
        offset = target.transform.forward * 2;
        this.transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset, ref velocity, smoothTime);
    }
    void FixedUpdate()
    {
        if (left.isGrip && right.isGrip)
        {
            control();
        }
    }
}
