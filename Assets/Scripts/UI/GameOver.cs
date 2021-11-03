using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    private Controller leftController;
    private Controller rightController;

    private void OnEnable()
    {
        leftController = GameObject.Find("Left Controller").GetComponent<Controller>();
        rightController = GameObject.Find("Right Controller").GetComponent<Controller>();
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (leftController.isTrigger) Menu();
        if (rightController.isTrigger) Retry();
    }

    private void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Menu()
    {
        SceneManager.LoadScene("Menu Scene");
    }
}
