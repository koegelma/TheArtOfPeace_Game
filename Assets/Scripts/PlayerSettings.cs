using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public GameObject playerCalibration;
    public GameObject calibrationUI;
    public GameObject mainMenuUI;
    public Menu menuScript;
    private void Awake()
    {
        if (!GameData.isPlayerInitialized) ToggleCalibration();
    }

    public void ToggleCalibration()
    {
        menuScript.isSettings = !menuScript.isSettings;
        mainMenuUI.SetActive(!mainMenuUI.activeSelf);
        playerCalibration.SetActive(!playerCalibration.activeSelf);
        calibrationUI.SetActive(!calibrationUI.activeSelf);
    }
}
