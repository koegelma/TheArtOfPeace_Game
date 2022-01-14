using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour
{
    private Controller leftController;
    private Controller rightController;
    public GameObject menuUI;
    public GameObject optionsUI;
    public GameObject calibrationUI;
    public GameObject playerCalibration;
    public bool isOptions = false;
    public bool isCalibration = false;
    private bool isLeftTriggerReady = true;
    private bool isRightTriggerReady = true;
    public string sceneToLoad = "Testing Scene";
    public AudioSource uiClick;
    private void Start()
    {
        Time.timeScale = 1f;
        leftController = GameObject.Find("Left Controller").GetComponent<Controller>();
        rightController = GameObject.Find("Right Controller").GetComponent<Controller>();
    }

    private void Update()
    {
        if (isCalibration) return;
        if (!isLeftTriggerReady && !leftController.isTrigger) isLeftTriggerReady = true;
        if (!isRightTriggerReady && !rightController.isTrigger) isRightTriggerReady = true;
        if (isOptions)
        {
            if (leftController.isTrigger && isLeftTriggerReady)
            {
                uiClick.Play();
                ToggleOptions();
                ToggleMenu();
                isLeftTriggerReady = false;
            }
            if (rightController.isTrigger && isRightTriggerReady)
            {
                uiClick.Play();
                ToggleOptions();
                ToggleCalibration();
                isRightTriggerReady = false;
            }
            return;
        }
        if (leftController.isTrigger && isLeftTriggerReady)
        {
            uiClick.Play();
            ToggleMenu();
            ToggleOptions();
            isLeftTriggerReady = false;
        }
        if (rightController.isTrigger) PrepareNewGame();
    }

    private void PrepareNewGame()
    {
        uiClick.Play();
        StartCoroutine(NewGame());
    }

    private IEnumerator NewGame()
    {
        yield return new WaitUntil(() => !uiClick.isPlaying);
        SceneManager.LoadScene(sceneToLoad);
    }

    private void ToggleOptions()
    {
        isOptions = !isOptions;
        optionsUI.SetActive(!optionsUI.activeSelf);
    }

    public void ToggleMenu()
    {
        menuUI.SetActive(!menuUI.activeSelf);
    }

    public void ToggleCalibration()
    {
        isCalibration = !isCalibration;
        playerCalibration.SetActive(!playerCalibration.activeSelf);
        calibrationUI.SetActive(!calibrationUI.activeSelf);
    }
}
