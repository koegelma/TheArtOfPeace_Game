using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public Menu menuScript;
    private void Awake()
    {
        if (!GameData.isPlayerInitialized)
        {
            menuScript.ToggleMenu();
            menuScript.ToggleCalibration();
        }
    }
}
