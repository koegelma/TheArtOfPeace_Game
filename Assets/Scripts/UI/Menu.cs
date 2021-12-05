using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private Controller leftController;
    private Controller rightController;
    public bool isSettings = false;
    public string sceneToLoad = "Testing Scene";
    private void Start()
    {
        Time.timeScale = 1f;
        leftController = GameObject.Find("Left Controller").GetComponent<Controller>();
        rightController = GameObject.Find("Right Controller").GetComponent<Controller>();
    }

    private void Update()
    {
        if (isSettings) return;
        if (leftController.isTrigger) Options();
        if (rightController.isTrigger) NewGame();
    }

    private void NewGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    private void Options()
    {
        Debug.Log("Options");
    }
}
