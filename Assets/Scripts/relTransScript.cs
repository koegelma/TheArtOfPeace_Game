using UnityEngine;

public class relTransScript : MonoBehaviour
{
    public GameObject leftCube;
    public GameObject rightCube;
    public Controller leftController;
    public Controller rightController;
    // Update is called once per frame
    void Update()
    {
        leftCube.transform.position = leftController.relativeTransformNoAngles.position + new Vector3(0,1,2);
        rightCube.transform.position = rightController.relativeTransformNoAngles.position + new Vector3(0,1,2);
    }
}
