using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    EASY, MEDIUM, HARD
}
public class OrbManager : MonoBehaviour
{
    public static OrbManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one OrbManager in scene!");
            return;
        }
        instance = this;
    }

    public List<GameObject> orbs = new List<GameObject>();

    private OrbMovement orbMovement;
    public int orbsCreated = 0;

    public bool HasOrbs { get { return orbs.Count >= 1; } }
    public int OrbsInGame { get { return orbs.Count; } }

    public void AddOrb(GameObject _orb)
    {
        orbs.Add(_orb);
        orbsCreated++;
        //Debug.Log("Orb added");
    }

    public void RemoveOrb(GameObject _orb)
    {
        foreach (GameObject orb in orbs)
        {
            if (orb.name == _orb.name)
            {
                orbs.Remove(orb);
                //Debug.Log("Orb removed");
                return;
            }
        }
    }

    public bool IsAnyOrbDirectedAtPlayer()
    {
        foreach (GameObject orb in orbs)
        {
            orbMovement = orb.GetComponent<OrbMovement>();
            if (orbMovement.targetIsPlayer) return true;
        }
        return false;
    }

    public GameObject GetOrbDirectedAtPlayer()
    {
        float distanceToPlayer = Mathf.Infinity;
        GameObject orbDirectedAtPlayer = null;
        foreach (GameObject orb in orbs)
        {
            orbMovement = orb.GetComponent<OrbMovement>();
            if (orbMovement.targetIsPlayer && orbMovement.GetDistanceToTarget() < distanceToPlayer)
            {
                distanceToPlayer = orbMovement.GetDistanceToTarget();
                orbDirectedAtPlayer = orb;
            }
        }
        return orbDirectedAtPlayer;
    }

    public List<GameObject> GetAllOrbsDirectedAtPlayer()
    {
        List<GameObject> orbsDirectedAtPlayer = new List<GameObject>();
        foreach (GameObject orb in orbs)
        {
            orbMovement = orb.GetComponent<OrbMovement>();
            if (orbMovement.targetIsPlayer) orbsDirectedAtPlayer.Add(orb);
        }
        return orbsDirectedAtPlayer;
    }
}
