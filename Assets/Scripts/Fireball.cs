using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fireball : MonoBehaviour, I_Spell
{
    public static Phase[] phases = new Phase[2];
    public static float manaCost = 30;
    public GameObject fireballPrefab;
    void OnCollisionEnter(Collision other)
    {
        Destroy(this.fireballPrefab);
        Destroy(this);
        
        if(other.gameObject.GetComponent<Damageable>()){   
            other.gameObject.GetComponent<Damageable>().takeDamage(30);
        }
    }

    public static bool checkPhase(GameObject relativeControllerTransformLeft, GameObject relativeControllerTransformRight, float tolerance, int phase){
        phases[0] = new Phase((new Vector3(-0.2f,-0.1f,0.5f)), (new Vector3(0.1f,-0.3f,0.1f)));
        phases[1] = new Phase((new Vector3(-0.1f,-0.2f,0.0f)), (new Vector3(0.1f,-0.1f,0.6f)));
        if((relativeControllerTransformLeft.transform.position.x < Fireball.phases[phase].left.x + tolerance && relativeControllerTransformLeft.transform.position.x > Fireball.phases[phase].left.x - tolerance
            && relativeControllerTransformLeft.transform.position.y < Fireball.phases[phase].left.y + tolerance && relativeControllerTransformLeft.transform.position.y > Fireball.phases[phase].left.y - tolerance
            && relativeControllerTransformLeft.transform.position.z < Fireball.phases[phase].left.z + tolerance && relativeControllerTransformLeft.transform.position.z > Fireball.phases[phase].left.z - tolerance)
            && // Relative position of the Left&Right controller have to be on startingPoint/phase Vectors +- tolerance
               (relativeControllerTransformRight.transform.position.x < Fireball.phases[phase].right.x + tolerance && relativeControllerTransformRight.transform.position.x > Fireball.phases[phase].right.x - tolerance
            && relativeControllerTransformRight.transform.position.y < Fireball.phases[phase].right.y + tolerance && relativeControllerTransformRight.transform.position.y > Fireball.phases[phase].right.y - tolerance
            && relativeControllerTransformRight.transform.position.z < Fireball.phases[phase].right.z + tolerance && relativeControllerTransformRight.transform.position.z > Fireball.phases[phase].right.z - tolerance)){
                return true;
            }
        return false;
    }
}