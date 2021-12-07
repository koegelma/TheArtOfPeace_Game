using UnityEngine;

public static class GameData
{
    public static bool isPlayerInitialized;
    public static float armLengthFactor;

    static GameData()
    {
        armLengthFactor = 1;
        isPlayerInitialized = false;
    }

    public static Vector3[] CalcPlayerPhaseCoords(Vector3[] armPhaseCoords)
    {
        Vector3[] newArmPhaseCoords = new Vector3[armPhaseCoords.Length];
        int phaseIndex = 0;
        foreach (Vector3 phaseCoord in armPhaseCoords)
        {
            newArmPhaseCoords[phaseIndex].x = armPhaseCoords[phaseIndex].x * armLengthFactor;
            newArmPhaseCoords[phaseIndex].y = armPhaseCoords[phaseIndex].y;
            newArmPhaseCoords[phaseIndex].z = armPhaseCoords[phaseIndex].z * armLengthFactor;
            phaseIndex++;
        }
        return newArmPhaseCoords;
    }
}
