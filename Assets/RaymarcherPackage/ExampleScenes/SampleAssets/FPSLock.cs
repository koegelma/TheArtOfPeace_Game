using UnityEngine;

/// <summary>
/// FPS lock & counter - in dev purposes
/// </summary>
public class FPSLock : MonoBehaviour
{
    public bool showFPS = true;

    private float timer;
    private readonly float refresh;
    private float avgFramerate;
    private readonly string display = "{0} FPS";
    private string tex;

    private void Awake()
    {
        Application.targetFrameRate = 300;
    }

    private void Update()
    {
        if (!showFPS) return;
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        tex = string.Format(display, avgFramerate.ToString());
    }

    private void OnGUI()
    {
        if (!showFPS) return;
        GUILayout.BeginArea(new Rect(Screen.width / 2, 20, 500, 300));
        GUILayout.Label(tex);
        GUILayout.EndArea();
    }
}