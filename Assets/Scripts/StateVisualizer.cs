using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateVisualizer : MonoBehaviour
{
    public GameObject smallCube;
    // Update is called once per frame

    void Update()
    {
        switch (StateManager.state)
        {
            case State.IDLE:
                this.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
                break;
            case State.PATTERN1:
                this.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                break;
            case State.PATTERN2:
                this.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                break;
        }
        switch (StateManager.currentPhase)
        {
            case -1:
                smallCube.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                break;
            case 0:
                smallCube.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
                break;
            case 1:
                smallCube.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                break;
            case 2:
                smallCube.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                break;
        }
        
    }
}
