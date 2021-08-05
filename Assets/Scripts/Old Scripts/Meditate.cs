/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meditate : MonoBehaviour
{    
    
    public static float manaCost = 0;
    public static bool isFluid = false;



    public static bool checkPhase(GameObject relativeControllerTransformLeft, GameObject relativeControllerTransformRight, float velocityLeft, float velocityRight, float tolerance, int phase){
        phases[0] = new Phase((new Vector3(-0.4f,-0.1f,0.4f)), (new Vector3(0.4f,-0.1f,0.4f)));
        phases[1] = new Phase((new Vector3(-0.2f,-0.2f,0.1f)), (new Vector3(0.2f,-0.2f,0.1f)));
        phases[2] = new Phase((new Vector3(-0.4f,-0.1f,0.4f)), (new Vector3(0.4f,-0.1f,0.4f)));
        if((relativeControllerTransformLeft.transform.position.x < Meditate.phases[phase].left.x + tolerance && relativeControllerTransformLeft.transform.position.x > Meditate.phases[phase].left.x - tolerance
            && relativeControllerTransformLeft.transform.position.y < Meditate.phases[phase].left.y + tolerance && relativeControllerTransformLeft.transform.position.y > Meditate.phases[phase].left.y - tolerance
            && relativeControllerTransformLeft.transform.position.z < Meditate.phases[phase].left.z + tolerance && relativeControllerTransformLeft.transform.position.z > Meditate.phases[phase].left.z - tolerance)
            && // Relative position of the Left&Right controller have to be on startingPoint/phase Vectors +- tolerance
               (relativeControllerTransformRight.transform.position.x < Meditate.phases[phase].right.x + tolerance && relativeControllerTransformRight.transform.position.x > Meditate.phases[phase].right.x - tolerance
            && relativeControllerTransformRight.transform.position.y < Meditate.phases[phase].right.y + tolerance && relativeControllerTransformRight.transform.position.y > Meditate.phases[phase].right.y - tolerance
            && relativeControllerTransformRight.transform.position.z < Meditate.phases[phase].right.z + tolerance && relativeControllerTransformRight.transform.position.z > Meditate.phases[phase].right.z - tolerance)){
                return true;
            }
        int maxSpeed = 1;
        float minSpeed = 0.1f;
        Debug.Log(velocityLeft);
        if(velocityLeft > maxSpeed || velocityRight > maxSpeed || velocityLeft < minSpeed || velocityRight < minSpeed ){
            isFluid = true;
            Debug.Log("Not so fast champ");
        }
        return false;
    }
}
 */