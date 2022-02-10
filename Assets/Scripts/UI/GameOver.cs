using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    private Controller leftController;
    private Controller rightController;
    public AudioSource uiClick;

    private void OnEnable()
    {
        leftController = GameObject.Find("Left Controller").GetComponent<Controller>();
        rightController = GameObject.Find("Right Controller").GetComponent<Controller>();
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (leftController.isTrigger) PrepareMenu();
        if (rightController.isTrigger) PrepareRetry();
    }

    private void PrepareRetry()
    {
        uiClick.Play();
        StartCoroutine(Retry());
    }

    private void PrepareMenu()
    {
        uiClick.Play();
        StartCoroutine(Menu());
    }

    private IEnumerator Retry()
    {
        yield return new WaitUntil(() => !uiClick.isPlaying);
        SceneManager.LoadScene("Testing Scene");
    }

    private IEnumerator Menu()
    {
        yield return new WaitUntil(() => !uiClick.isPlaying);
        SceneManager.LoadScene("Menu Scene");
    }
}
