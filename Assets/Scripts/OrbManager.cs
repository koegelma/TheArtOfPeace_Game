using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool HasOrbs { get { return orbs.Count >= 1; } }

    public void AddOrb(GameObject _orb)
    {
        foreach (GameObject orb in orbs)
        {
            if (_orb.name == orb.name)
            {
                Debug.Log("orb already in orbs");
                return;
            }
            orbs.Add(_orb);
        }
    }

    public void RemoveOrb(GameObject _orb)
    {
        foreach (GameObject orb in orbs)
        {
            if (orb.name == _orb.name)
            {
                orbs.Remove(orb);
                return;
            }
        }
    }

    public bool IsAnyOrbDirectedAtPlayer()
    {
        foreach (GameObject orb in orbs)
        {
            orbMovement = orb.GetComponent<OrbMovement>();
            if (orbMovement.TargetIsPlayer) return true;
        }
        return false;
    }
    public GameObject GetOrbDirectedAtPlayer()
    {
        float distanceToPlayer = 0;
        GameObject orbDirectedAtPlayer = null;
        foreach (GameObject orb in orbs)
        {
            orbMovement = orb.GetComponent<OrbMovement>();
            if (orbMovement.TargetIsPlayer && orbMovement.GetDistanceToPlayer() > distanceToPlayer)
            {
                distanceToPlayer = orbMovement.GetDistanceToPlayer();
                orbDirectedAtPlayer = orb;
            }
        }
        return orbDirectedAtPlayer;
    }

}
