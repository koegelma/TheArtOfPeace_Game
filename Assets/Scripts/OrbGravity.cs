using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbGravity : MonoBehaviour
{

    private OrbManager orbManager;
    public int pull;
    public int range;
    public GameObject easyOrbPrefab;
    public GameObject mediumOrbPrefab;
    public GameObject hardOrbPrefab;
    private OrbMovement orbMovement;
    // Start is called before the first frame update
    void Start()
    {
        orbManager = OrbManager.instance;
        orbMovement = GetComponent<OrbMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();
    }
    public void ApplyGravity()
    {
        if (orbMovement.tier != Difficulty.HARD)
        {
            List<GameObject> tempOrbList = new List<GameObject>();
            foreach (GameObject orb in orbManager.orbs)
            {
                Vector3 vectorFromOrbToThis = orb.transform.position - transform.position;
                if ((name != orb.name && vectorFromOrbToThis.magnitude < range) && orb.GetComponent<OrbMovement>().target == orbMovement.target && (orbMovement.targets.Length <= 1 || orbMovement.target.parent.GetComponent<PatternTarget>().isEnemyPattern))
                //&& vectorFromOrbToThis.magnitude > 0.6)
                {
                    tempOrbList.Add(orb);
                }
            }
            Vector3 sumGravityVector = new Vector3(0, 0, 0);
            foreach (GameObject gravOrb in tempOrbList)
            {
                sumGravityVector += (gravOrb.transform.position - transform.position);
            }
            if (tempOrbList.Count > 0)
            {
                sumGravityVector /= tempOrbList.Count;
            }
            GetComponent<Rigidbody>().AddForce(sumGravityVector * pull);
            CheckGroups(tempOrbList);
        }
    }


    public void CheckGroups(List<GameObject> tempOrbList)
    {
        if (orbMovement.tier != Difficulty.HARD)
        {
            List<GameObject> mergeList = new List<GameObject>();
            foreach (GameObject orb in tempOrbList)
            {
                if (((orb.transform.position - transform.position).magnitude < 0.7) && orb.transform.parent == null && orb.name != name && orb.GetComponent<OrbMovement>().tier == Difficulty.EASY)
                {
                    mergeList.Add(orb);
                }
            }
            if (mergeList.Count > 0)
            {
                GameObject newOrb = null;
                if (mergeList.Count == 1)
                {
                    orbManager.RemoveOrb(gameObject);
                    Destroy(gameObject);
                    orbManager.RemoveOrb(mergeList[0]);
                    Destroy(mergeList[0]);

                    if (orbMovement.tier == Difficulty.EASY)
                    {
                        newOrb = Instantiate(mediumOrbPrefab, transform.position, transform.rotation);
                    }
                    else
                    {
                        newOrb = Instantiate(hardOrbPrefab, transform.position, transform.rotation);
                    }
                }
                else
                {
                    if (orbMovement.tier == Difficulty.EASY)
                    {
                        newOrb = Instantiate(hardOrbPrefab, transform.position, transform.rotation);
                        orbManager.RemoveOrb(gameObject);
                        Destroy(gameObject);
                        orbManager.RemoveOrb(mergeList[0]);
                        Destroy(mergeList[0]);
                        orbManager.RemoveOrb(mergeList[1]);
                        //UpdatePatternOrbList(groupList, newOrb);
                        Destroy(mergeList[1]);
                    }
                    else if (orbMovement.tier == Difficulty.MEDIUM)
                    {
                        newOrb = Instantiate(hardOrbPrefab, transform.position, transform.rotation);
                        orbManager.RemoveOrb(gameObject);
                        Destroy(gameObject);
                        orbManager.RemoveOrb(mergeList[0]);
                        //UpdatePatternOrbList(groupList, newOrb);
                        Destroy(mergeList[0]);
                    }
                }
                MergeMovement(orbMovement, newOrb.GetComponent<OrbMovement>());
            }
        }
    }

    /*    private void UpdatePatternOrbList(List<GameObject> oldOrbs, GameObject newOrb)
       {
           oldOrbs.Add(gameObject);

           List<GameObject> orbs = PatternManager.instance.activePattern.orbsDirectedAtPlayer;
           foreach (GameObject orb in oldOrbs)
           {
               if (orbs.Contains(orb)) orbs.Remove(orb);
           }
           orbs.Add(newOrb);
       } */

    private void MergeMovement(OrbMovement original, OrbMovement merged)
    {
        if (merged != null)
        {
            //Debug.Log("merged!");
            merged.targets = original.targets;
            merged.target = original.target;
            merged.targetIndex = original.targetIndex;
            merged.isFinalEnemyTargetPassed = original.isFinalEnemyTargetPassed;
            merged.isFinalPlayerTargetPassed = original.isFinalPlayerTargetPassed;
            merged.targetIsController = original.targetIsController;
            merged.isMerged = true;
        }
    }
}
