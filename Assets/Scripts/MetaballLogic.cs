using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaballLogic : MonoBehaviour
{
    public bool X = true, Y = true, Z = true;
    public float speed = 2.0f;
    public float amount = 3.0f;

    private void Update()
    {
        Vector3 p = transform.position;
        p.x = X ? Mathf.Sin(Time.time * speed) * amount : p.x;
        p.y = Y ? Mathf.Cos(Time.time * speed) * amount : p.y;
        p.z = Z ? 0.124892f + Mathf.Sin(Time.time * speed) * amount : p.z;
        transform.position = p;
    }
}