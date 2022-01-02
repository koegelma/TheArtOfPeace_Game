using UnityEngine;

public class PatternManager : MonoBehaviour
{
    public static PatternManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PatternManager in scene!");
            return;
        }
        instance = this;
    }
   

   
}
