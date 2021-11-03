using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static int life;
    private int startLife = 100;
    public Text lifeText;
    public static int targetIndex;
    public Text targetIndexText;
    public static int phase;
    public Text phaseText;


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

        phase = StateManager.currentPhase;
        phaseText.text = phase.ToString();

        if (!OrbManager.instance.IsAnyOrbDirectedAtPlayer())
        {
            targetIndexText.text = targetIndex.ToString();
            return;
        }

        OrbMovement orbScript = OrbManager.instance.GetOrbDirectedAtPlayer().GetComponent<OrbMovement>();
        targetIndex = orbScript.targetIndex;
        targetIndexText.text = targetIndex.ToString();

    }
}
