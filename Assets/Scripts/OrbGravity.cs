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
    // Start is called before the first frame update
    void Start()
    {
        orbManager = OrbManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();

    }
    public void ApplyGravity()
    {
        if (GetComponent<OrbMovement>().tier != Difficulty.HARD)
        {
            List<GameObject> tempOrbList = new List<GameObject>();
            foreach (GameObject orb in orbManager.orbs)
            {
                Vector3 vectorFromiOrbToOrb = orb.transform.position - transform.position;
                if ((name != orb.name && vectorFromiOrbToOrb.magnitude < range) && vectorFromiOrbToOrb.magnitude > 0.6)
                {
                    tempOrbList.Add(orb);
                }
            }
            Vector3 sumGravityVector = new Vector3(0, 0, 0);
            foreach (GameObject gravOrb in tempOrbList)
            {
                if (gravOrb.transform.parent == transform.parent)
                {
                    sumGravityVector += (gravOrb.transform.position - transform.position) * 2;
                }
                else
                {
                    sumGravityVector += (gravOrb.transform.position - transform.position);
                }
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
        if (GetComponent<OrbMovement>().tier != Difficulty.HARD)
        {
            List<GameObject> groupList = new List<GameObject>();
            foreach (GameObject orb in tempOrbList)
            {
                if (((orb.transform.position - transform.position).magnitude < 0.7) && orb.transform.parent == null && orb.name != name)
                {
                    groupList.Add(orb);
                }
            }
            if (groupList.Count > 0)
            {
                GameObject newOrb = null;
                if (groupList.Count == 1)
                {
                    orbManager.RemoveOrb(gameObject);
                    Destroy(gameObject);
                    orbManager.RemoveOrb(groupList[0]);
                    Destroy(groupList[0]);

                    if (GetComponent<OrbMovement>().tier == Difficulty.EASY)
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
                    if (GetComponent<OrbMovement>().tier == Difficulty.EASY)
                    {
                        orbManager.RemoveOrb(gameObject);
                        Destroy(gameObject);
                        orbManager.RemoveOrb(groupList[0]);
                        Destroy(groupList[0]);
                        orbManager.RemoveOrb(groupList[1]);
                        Destroy(groupList[1]);
                        newOrb = Instantiate(hardOrbPrefab, transform.position, transform.rotation);
                    }
                    else if (GetComponent<OrbMovement>().tier == Difficulty.MEDIUM)
                    {
                        orbManager.RemoveOrb(gameObject);
                        Destroy(gameObject);
                        orbManager.RemoveOrb(groupList[0]);
                        Destroy(groupList[0]);
                        newOrb = Instantiate(hardOrbPrefab, transform.position, transform.rotation);
                    }
                }
                MergeMovement(GetComponent<OrbMovement>(), newOrb.GetComponent<OrbMovement>());
            }
        }
    }
    private void MergeMovement(OrbMovement original, OrbMovement merged)
    {
        if (merged != null)
        {
            Debug.Log("merged!");
            merged.targets = original.targets;
            merged.target = original.target;
            merged.targetIndex = original.targetIndex;
            merged.isFinalEnemyTargetPassed = original.isFinalEnemyTargetPassed;
            merged.isFinalPlayerTargetPassed = original.isFinalPlayerTargetPassed;
        }
    }
}
