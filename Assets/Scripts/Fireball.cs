using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour, I_Spell
{
    public static Vector3 startingPointLeft = new Vector3(-0.2f,-0.1f,0.5f);
    public static Vector3 startingPointRight = new Vector3(0.1f,-0.3f,0.1f);

    public static Vector3 phase2Left = new Vector3(-0.1f,-0.2f,0.0f);
    public static Vector3 phase2Right = new Vector3(0.1f,-0.1f,0.5f);
    public GameObject fireballPrefab;
    public GameObject test;

    public void run(Vector3 relativeControllerPositionRight)
    {
        //Resources.Load("FireballPrefab")
        //We cannot find the GameObject Prefab, since its not in the scene
        Instantiate(GameObject.Find("FireballPrefab") , relativeControllerPositionRight, Quaternion.identity);
        test = GameObject.Find("FireballPrefab(Clone)");
   
        if (test == null){

        }
    }
}
