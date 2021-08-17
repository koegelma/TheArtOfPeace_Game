
using UnityEngine;

public class PhaseChecker
{
    public Controller leftController;
    public Controller rightController;
    Vector3[] leftPhaseCoords;
    Vector3[] rightPhaseCoords;
    public static float tolerance;


    public PhaseChecker(Vector3[] leftPhaseCoords, Vector3[] rightPhaseCoords){
        this.leftPhaseCoords = leftPhaseCoords;
        this.rightPhaseCoords = rightPhaseCoords;
        this.leftController = GameObject.Find("LeftController").GetComponent<Controller>();
        this.rightController = GameObject.Find("RightController").GetComponent<Controller>();
        tolerance = 0.2f;
    }

    public bool check(int phase)
    {
        if(leftController.isTrigger && rightController.isTrigger){
            if ((leftController.relativeTransform.position.x < leftPhaseCoords[phase].x + tolerance && leftController.relativeTransform.position.x > leftPhaseCoords[phase].x - tolerance
            && leftController.relativeTransform.position.y < leftPhaseCoords[phase].y + tolerance && leftController.relativeTransform.position.y > leftPhaseCoords[phase].y - tolerance
            && leftController.relativeTransform.position.z < leftPhaseCoords[phase].z + tolerance && leftController.relativeTransform.position.z > leftPhaseCoords[phase].z - tolerance)
            && // Relative position of the Left&Right controller have to be on startingPoint/phase Vectors +- tolerance
               (rightController.relativeTransform.position.x < rightPhaseCoords[phase].x + tolerance && rightController.relativeTransform.position.x > rightPhaseCoords[phase].x - tolerance
            && rightController.relativeTransform.position.y < rightPhaseCoords[phase].y + tolerance && rightController.relativeTransform.position.y > rightPhaseCoords[phase].y - tolerance
            && rightController.relativeTransform.position.z < rightPhaseCoords[phase].z + tolerance && rightController.relativeTransform.position.z > rightPhaseCoords[phase].z - tolerance))
            {
                //if (checkFluidity(0.1f, 1)) return false; 
                return true;
            }
        }
        return false;
    }   
    private bool checkFluidity(float minSpeed, float maxSpeed){
        float leftVelocityMagnitude = leftController.controllerVelocity.magnitude;
        float rightVelocityMagnitude = rightController.controllerVelocity.magnitude;
        if (leftVelocityMagnitude > maxSpeed || rightVelocityMagnitude > maxSpeed)
        {
            Debug.Log("Too fast.");
            return true;
        }
        if (leftVelocityMagnitude < minSpeed || rightVelocityMagnitude < minSpeed){
            Debug.Log("Too slow.");
            return true;
        }
        return false;
    }
}