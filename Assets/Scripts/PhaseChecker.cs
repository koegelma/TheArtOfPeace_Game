
using UnityEngine;

public class PhaseChecker
{
    public Controller leftController;
    public Controller rightController;
    Vector3[] leftPhaseCoords;
    Vector3[] rightPhaseCoords;
    public static float tolerance;

    public Vector3 tempRightPosition;
    public Vector3 tempLeftPosition;


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
            if ((leftController.relativeTransform.position.x < leftPhaseCoords[phase].x + tolerance && leftController.relativeTransform.position.x > leftPhaseCoords[phase].x - tolerance
            && leftController.relativeTransform.position.y < leftPhaseCoords[phase].y + tolerance && leftController.relativeTransform.position.y > leftPhaseCoords[phase].y - tolerance
            && leftController.relativeTransform.position.z < leftPhaseCoords[phase].z + tolerance && leftController.relativeTransform.position.z > leftPhaseCoords[phase].z - tolerance)
            && // Relative position of the Left&Right controller have to be on startingPoint/phase Vectors +- tolerance
               (rightController.relativeTransform.position.x < rightPhaseCoords[phase].x + tolerance && rightController.relativeTransform.position.x > rightPhaseCoords[phase].x - tolerance
            && rightController.relativeTransform.position.y < rightPhaseCoords[phase].y + tolerance && rightController.relativeTransform.position.y > rightPhaseCoords[phase].y - tolerance
            && rightController.relativeTransform.position.z < rightPhaseCoords[phase].z + tolerance && rightController.relativeTransform.position.z > rightPhaseCoords[phase].z - tolerance))
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
            GameObject tempLeftPhaseCoord = new GameObject("tempLeftPhaseCoord");
            GameObject tempRightPhaseCoord = new GameObject("tempRightPhaseCoord");

            tempLeftPhaseCoord.transform.position = leftPhaseCoords[phase];
            tempRightPhaseCoord.transform.position = rightPhaseCoords[phase];


            // point rotates around itself, not around camera, why? --> offset on xrrig and camera in z direction, but why this?
            tempLeftPhaseCoord.transform.RotateAround(leftController.GetMainCamera().transform.position, Vector3.up, 180);
            //leftController.GetMainCamera().transform.eulerAngles.y);
            tempRightPhaseCoord.transform.RotateAround(leftController.GetMainCamera().transform.position, Vector3.up, 180);
            //leftController.GetMainCamera().transform.eulerAngles.y);

            tempLeftPosition = tempRightPhaseCoord.transform.position;
            tempRightPosition = tempLeftPhaseCoord.transform.position;

            if ((leftController.nextRelativeTransform.position.x < tempLeftPhaseCoord.transform.position.x + tolerance && leftController.nextRelativeTransform.position.x > tempLeftPhaseCoord.transform.position.x - tolerance
            && leftController.nextRelativeTransform.position.y < tempLeftPhaseCoord.transform.position.y + tolerance && leftController.nextRelativeTransform.position.y > tempLeftPhaseCoord.transform.position.y - tolerance
            && leftController.nextRelativeTransform.position.z < tempLeftPhaseCoord.transform.position.z + tolerance && leftController.nextRelativeTransform.position.z > tempLeftPhaseCoord.transform.position.z - tolerance)
            &&
               (rightController.nextRelativeTransform.position.x < tempRightPhaseCoord.transform.position.x + tolerance && rightController.nextRelativeTransform.position.x > tempRightPhaseCoord.transform.position.x - tolerance
            && rightController.nextRelativeTransform.position.y < tempRightPhaseCoord.transform.position.y + tolerance && rightController.nextRelativeTransform.position.y > tempRightPhaseCoord.transform.position.y - tolerance
            && rightController.nextRelativeTransform.position.z < tempRightPhaseCoord.transform.position.z + tolerance && rightController.nextRelativeTransform.position.z > tempRightPhaseCoord.transform.position.z - tolerance))
            {
                GameObject.Destroy(tempLeftPhaseCoord);
                GameObject.Destroy(tempRightPhaseCoord);
                return true;
            }
            GameObject.Destroy(tempLeftPhaseCoord);
            GameObject.Destroy(tempRightPhaseCoord);
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