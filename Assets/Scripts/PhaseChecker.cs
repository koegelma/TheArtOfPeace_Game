/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// we need:
// Spell Timestamps, Spell Cooldowns, 
public class PhaseChecker : MonoBehaviour
{
    public static void checkPhases(GameObject relativeControllerTransformLeft, GameObject relativeControllerTransformRight)
    {
        float tolerance = 0.2f;
        if (I_Spell.timestamp <= Time.time || state == State.FIREBALL)
        {
            if (Fireball.checkPhase(relativeControllerTransformLeft, relativeControllerTransformRight, tolerance, 0) || state == State.FIREBALL)
            {
                SpellManager.state = State.FIREBALL;
                stateTimestamp = Time.time;
                if (Fireball.checkPhase(relativeControllerTransformLeft, relativeControllerTransformRight, tolerance, 1))
                {
                    GameObject fireballInstance = Instantiate(fireballPrefab, controllerPositionRight, controllerRotationRight);
                    fireballInstance.GetComponent<Rigidbody>().AddForce(fireballInstance.transform.forward * 80);
                    fireballTimestamp = Time.time + fireballCooldownInSeconds;
                    state = State.IDLE;
                }
            }
        }
    }
} */