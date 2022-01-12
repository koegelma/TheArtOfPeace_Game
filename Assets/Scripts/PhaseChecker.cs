
using UnityEngine;

public class PhaseChecker
{
    public Controller leftController;
    public Controller rightController;
    Vector3[] leftPhaseCoords;
    Vector3[] rightPhaseCoords;
    public static float tolerance;
    public Vector3 globalRightPhaseCoord;
    public Vector3 globalLeftPhaseCoord;
    public Vector3 globalRightControllerPosition;
    public Vector3 globalLeftControllerPosition;


    public PhaseChecker(Vector3[] leftPhaseCoords, Vector3[] rightPhaseCoords)
    {
        this.leftPhaseCoords = leftPhaseCoords;
        this.rightPhaseCoords = rightPhaseCoords;
        this.leftController = GameObject.Find("Left Controller").GetComponent<Controller>();
        this.rightController = GameObject.Find("Right Controller").GetComponent<Controller>();
        tolerance = 0.07f;
    }

    public bool FirstCheck(int phase)
    {
        if (leftController.isTrigger && rightController.isTrigger)
        {
            Debug.Log("LeftController: " + leftController.controllerPosition);
            Debug.Log("LeftPhaseCoord: " + globalLeftPhaseCoord);
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

    public bool NextCheck(int phase) //, Vector3 leftCoord, Vector3 rightCoord
    {
        // global controller positions benutzen
        if (leftController.isTrigger && rightController.isTrigger)
        {
            /* GameObject tempLeftPhaseCoord = new GameObject("tempLeftPhaseCoord");
            GameObject tempRightPhaseCoord = new GameObject("tempRightPhaseCoord");

            tempLeftPhaseCoord.transform.position = leftPhaseCoords[phase];
            tempRightPhaseCoord.transform.position = rightPhaseCoords[phase];


            // point rotates around itself, not around camera, why? --> offset on xrrig and camera in z direction, but why this?
            tempLeftPhaseCoord.transform.RotateAround(leftController.GetMainCamera().transform.position, Vector3.up, leftController.GetMainCamera().transform.eulerAngles.y);
            tempRightPhaseCoord.transform.RotateAround(leftController.GetMainCamera().transform.position, Vector3.up, leftController.GetMainCamera().transform.eulerAngles.y);

            tempLeftPosition = tempRightPhaseCoord.transform.position;
            tempRightPosition = tempLeftPhaseCoord.transform.position; */

            Debug.Log("LeftController: " + leftController.controllerPosition);
            Debug.Log("LeftPhaseCoord: " + globalLeftPhaseCoord);

            if ((leftController.controllerPosition.x < globalLeftPhaseCoord.x + tolerance && leftController.controllerPosition.x > globalLeftPhaseCoord.x - tolerance
            && leftController.controllerPosition.y < globalLeftPhaseCoord.y + tolerance && leftController.controllerPosition.y > globalLeftPhaseCoord.y - tolerance
            && leftController.controllerPosition.z < globalLeftPhaseCoord.z + tolerance && leftController.controllerPosition.z > globalLeftPhaseCoord.z - tolerance)
            &&
               (rightController.controllerPosition.x < globalRightPhaseCoord.x + tolerance && rightController.controllerPosition.x > globalRightPhaseCoord.x - tolerance
            && rightController.controllerPosition.y < globalRightPhaseCoord.y + tolerance && rightController.controllerPosition.y > globalRightPhaseCoord.y - tolerance
            && rightController.controllerPosition.z < globalRightPhaseCoord.z + tolerance && rightController.controllerPosition.z > globalRightPhaseCoord.z - tolerance))
            {
                //GameObject.Destroy(tempLeftPhaseCoord);
                //GameObject.Destroy(tempRightPhaseCoord);
                return true;
            }
            //GameObject.Destroy(tempLeftPhaseCoord);
            //GameObject.Destroy(tempRightPhaseCoord);
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