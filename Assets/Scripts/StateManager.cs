using UnityEngine;
using UnityEngine.XR;

public enum State
{
    IDLE, PATTERN1, TURN, WAVES
}
public class StateManager : MonoBehaviour
{
    public static StateManager instance;
    public State state;
    public float stateTimestamp;
    public int currentPhase = -1;
    public float countdown;
    public bool isFinalPhase;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one StateManager in scene!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        state = State.IDLE;
        isFinalPhase = false;
    }

    private void Update()
    {
        if (state != State.IDLE && !isFinalPhase) updateCountdown();
    }

    public int assurePhase(int phaseGiven)
    {
        if (phaseGiven > currentPhase)
        {
            return phaseGiven;
        }
        return currentPhase;
    }
    public void resetState()
    {
        //Debug.Log("phase reset to -1");
        state = State.IDLE;
        currentPhase = -1;
        isFinalPhase = false;
    }

    public bool switchPhase(int phaseGiven, float countdownInSeconds)
    {
        if (phaseGiven > currentPhase)
        {
            //Debug.Log("switched to phase " + phaseGiven); // test
            resetCountdown(phaseGiven - 1, countdownInSeconds);
            currentPhase = assurePhase(phaseGiven);
            return true;
        }
        return false;
    }
    public void resetCountdown(int phaseGiven, float countdownInSeconds)
    {
        if (currentPhase == phaseGiven)
        {
            countdown = countdownInSeconds;
        }
    }
    public void updateCountdown()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            //Debug.Log("Times up.");
            resetState();
        }
    }
}