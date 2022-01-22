using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static int life; 
    private int startLife = 100;
    public Text lifeText;
    public static int phase;
    public Text phaseText; 
    public Text activePatternText;


    private void Start()
    {
        life = startLife; 
    }
 
    private void Update()
    {
        HndLife(); 
        DisplayDevText();
    }

    private void HndLife()
    {
        if (GameManager.instance.isGameOver) return;

        if (life <= 0)
        {
            GameManager.instance.EndGame();
            life = 0;
        }
    }

    private void DisplayDevText()
    {
        lifeText.text = life.ToString();

        phase = StateManager.instance.currentPhase;
        phaseText.text = phase.ToString();

        activePatternText.text = PatternManager.instance.activePattern.name;

    }
}
