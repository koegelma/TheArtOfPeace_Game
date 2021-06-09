using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public static List<Water> waterList;
    public GameObject target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    Vector3 offset;
    public void control(){
        offset = target.transform.forward*3;
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position+offset, ref velocity, smoothTime);
    }
    void FixedUpdate()
    {
    }
    void OnEnable(){
        if(waterList == null)
            waterList = new List<Water>();
        waterList.Add(this);
    }

    void OnDisable(){
        waterList.Remove(this);
    }

}
