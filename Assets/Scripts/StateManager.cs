using UnityEngine;
using UnityEngine.XR;

public enum State
{
    IDLE, PATTERN1, PATTERN2
}
public static class StateManager
{
    public static State state = State.IDLE;
    public static float stateTimestamp;
    public static int currentPhase = -1;
    public static float countdown;
    

    public static int assurePhase(int phaseGiven)
    {
        if (phaseGiven > currentPhase)
        {
            return phaseGiven;
        }
        return currentPhase;
    }
    public static void resetState(){
        StateManager.state = State.IDLE;
        StateManager.currentPhase = -1;
    }
    
    public static bool switchPhase(int phaseGiven, float countdownInSeconds){
        if(phaseGiven > currentPhase){
            Debug.Log("switched to phase " + phaseGiven); // test
            resetCountdown(phaseGiven-1, countdownInSeconds);
            currentPhase = assurePhase(phaseGiven);
            return true;
        }
        return false;
    }
    public static void resetCountdown(int phaseGiven, float countdownInSeconds){
        if(currentPhase == phaseGiven){
            countdown = countdownInSeconds;
        }        
    }
    public static void updateCountdown(){
        countdown -= Time.deltaTime;
        if(countdown <= 0f){
            Debug.Log("Times up.");
            resetState();
        }
    }
} 