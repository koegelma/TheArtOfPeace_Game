using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : I_Spell
{
    public static Vector3 startingPointLeft = new Vector3(-0.2f,-0.1f,0.6f);
    public static Vector3 startingPointRight = new Vector3(0.1f,-0.3f,0.1f);

    public static Vector3 phase2Left;
    public static Vector3 phase2Right;

    public Fireball(Vector3 relativeControllerPositionRight)
    {
        GameObject fireCube = new GameObject();
        fireCube.transform.position = relativeControllerPositionRight;
    }

}
