using UnityEngine;

public class relTransScript : MonoBehaviour
{
    public GameObject leftCube;
    public GameObject rightCube;
    public Controller leftController;
    public Controller rightController;
    public Transform cameraTransform;
    private Vector3 offset = new Vector3(0, 1, 2);

    void Update()
    {
        /*  leftCube.transform.localPosition = leftController.transform.position + offset - cameraTransform.position;
         rightCube.transform.localPosition = rightController.transform.position + offset - cameraTransform.position; */


        //leftCube.transform.eulerAngles = new Vector3(leftCube.transform.eulerAngles.x, cameraTransform.eulerAngles.y, leftCube.transform.eulerAngles.z);
        //rightCube.transform.eulerAngles = new Vector3(rightCube.transform.eulerAngles.x, cameraTransform.eulerAngles.y, rightCube.transform.eulerAngles.z);

        //Vector3 pivotPoint = new Vector3(cameraTransform.position.x, 0, cameraTransform.position.z);

    
        leftCube.transform.localPosition = leftController.relativeTransform.position+offset;
        rightCube.transform.localPosition = rightController.relativeTransform.position+offset;

    }
}
