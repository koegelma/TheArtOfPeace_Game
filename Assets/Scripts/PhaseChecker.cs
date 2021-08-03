
using UnityEngine;

public class PhaseChecker
{
    public Controller leftController;
    public Controller rightController;
    Vector3[] leftPhaseCoords;
    Vector3[] rightPhaseCoords;
    float tolerance;


    public PhaseChecker(Vector3[] leftPhaseCoords, Vector3[] rightPhaseCoords){
        this.leftPhaseCoords = leftPhaseCoords;
        this.rightPhaseCoords = rightPhaseCoords;
        this.leftController = GameObject.Find("LeftController").GetComponent<Controller>();
        this.rightController = GameObject.Find("RightController").GetComponent<Controller>();
        this.tolerance = 0.2f;
    }

    public bool check(int phase)
    {
        if (checkFluidity(0.1f, 1)) return false; 
        if ((leftController.transform.position.x < leftPhaseCoords[phase].x + tolerance && leftController.transform.position.x > leftPhaseCoords[phase].x - tolerance
            && leftController.transform.position.y < leftPhaseCoords[phase].y + tolerance && leftController.transform.position.y > leftPhaseCoords[phase].y - tolerance
            && leftController.transform.position.z < leftPhaseCoords[phase].z + tolerance && leftController.transform.position.z > leftPhaseCoords[phase].z - tolerance)
            && // Relative position of the Left&Right controller have to be on startingPoint/phase Vectors +- tolerance
               (rightController.transform.position.x < rightPhaseCoords[phase].x + tolerance && rightController.transform.position.x > rightPhaseCoords[phase].x - tolerance
            && rightController.transform.position.y < rightPhaseCoords[phase].y + tolerance && rightController.transform.position.y > rightPhaseCoords[phase].y - tolerance
            && rightController.transform.position.z < rightPhaseCoords[phase].z + tolerance && rightController.transform.position.z > rightPhaseCoords[phase].z - tolerance))
        {
            return true;
        }
        return false;
    }
    private bool checkFluidity(float minSpeed, float maxSpeed){
        float leftVelocityMagnitude = leftController.controllerVelocity.magnitude;
        float rightVelocityMagnitude = rightController.controllerVelocity.magnitude;
        if (leftVelocityMagnitude > maxSpeed || rightVelocityMagnitude > maxSpeed || leftVelocityMagnitude < minSpeed || rightVelocityMagnitude < minSpeed)
        {
            return true;
        }
        return false;
    }
}