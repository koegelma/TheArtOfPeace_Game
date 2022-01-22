
using UnityEngine;

public class PhaseChecker
{
    public Controller leftController;
    public Controller rightController;
    Vector3[] leftPhaseCoords;
    Vector3[] rightPhaseCoords;
    public float tolerance;
    public Vector3 globalRightPhaseCoord;
    public Vector3 globalLeftPhaseCoord;
    public Vector3 globalRightControllerPosition;
    public Vector3 globalLeftControllerPosition; 


    public PhaseChecker(Vector3[] leftPhaseCoords, Vector3[] rightPhaseCoords, float _tolerance)
    {
        this.leftPhaseCoords = leftPhaseCoords; 
        this.rightPhaseCoords = rightPhaseCoords; 
        this.leftController = GameObject.Find("Left Controller").GetComponent<Controller>();
        this.rightController = GameObject.Find("Right Controller").GetComponent<Controller>();
        this.tolerance = _tolerance;
        //tolerance = 0.07f;
    }

    public bool FirstCheck(int phase) 
    {
        if (leftController.isTrigger && rightController.isTrigger)
        {
            if ((leftController.controllerPosition.x < globalLeftPhaseCoord.x + tolerance && leftController.controllerPosition.x > globalLeftPhaseCoord.x - tolerance
            && leftController.controllerPosition.y < globalLeftPhaseCoord.y + tolerance && leftController.controllerPosition.y > globalLeftPhaseCoord.y - tolerance
            && leftController.controllerPosition.z < globalLeftPhaseCoord.z + tolerance && leftController.controllerPosition.z > globalLeftPhaseCoord.z - tolerance)
            && // Relative position of the Left&Right controller have to be on startingPoint/phase Vectors +- tolerance
               (rightController.controllerPosition.x < globalRightPhaseCoord.x + tolerance && rightController.controllerPosition.x > globalRightPhaseCoord.x - tolerance
            && rightController.controllerPosition.y < globalRightPhaseCoord.y + tolerance && rightController.controllerPosition.y > globalRightPhaseCoord.y - tolerance
            && rightController.controllerPosition.z < globalRightPhaseCoord.z + tolerance && rightController.controllerPosition.z > globalRightPhaseCoord.z - tolerance))
            {
                //TODO: checkFluidity was da los?
                //if (checkFluidity(0.1f, 1)) return false;
                return true;
            } 
        }
        return false;
    }

    public bool NextCheck(int phase)
    {
        if (leftController.isTrigger && rightController.isTrigger)
        {
            if ((leftController.controllerPosition.x < globalLeftPhaseCoord.x + tolerance && leftController.controllerPosition.x > globalLeftPhaseCoord.x - tolerance
            && leftController.controllerPosition.y < globalLeftPhaseCoord.y + tolerance && leftController.controllerPosition.y > globalLeftPhaseCoord.y - tolerance
            && leftController.controllerPosition.z < globalLeftPhaseCoord.z + tolerance && leftController.controllerPosition.z > globalLeftPhaseCoord.z - tolerance)
            &&
               (rightController.controllerPosition.x < globalRightPhaseCoord.x + tolerance && rightController.controllerPosition.x > globalRightPhaseCoord.x - tolerance
            && rightController.controllerPosition.y < globalRightPhaseCoord.y + tolerance && rightController.controllerPosition.y > globalRightPhaseCoord.y - tolerance
            && rightController.controllerPosition.z < globalRightPhaseCoord.z + tolerance && rightController.controllerPosition.z > globalRightPhaseCoord.z - tolerance))
            {
                return true;
            }
        }
        return false;
    }

    private bool checkFluidity(float minSpeed, float maxSpeed)
    {
        float leftVelocityMagnitude = leftController.controllerVelocity.magnitude;
        float rightVelocityMagnitude = rightController.controllerVelocity.magnitude;
        if (leftVelocityMagnitude > maxSpeed || rightVelocityMagnitude > maxSpeed)
        {
            Debug.Log("Too fast.");
            return true;
        }
        if (leftVelocityMagnitude < minSpeed || rightVelocityMagnitude < minSpeed)
        {
            Debug.Log("Too slow.");
            return true;
        }
        return false;
    }
}