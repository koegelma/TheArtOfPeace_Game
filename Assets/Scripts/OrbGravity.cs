using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbGravity : MonoBehaviour
{

    private OrbManager orbManager;
    public int pull;
    public int range;
    
    // Start is called before the first frame update
    void Start()
    {
        orbManager = OrbManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        applyGravity();
        
    }
    public void applyGravity(){
        List<GameObject> tempOrbList = new List<GameObject>();
        foreach (GameObject orb in orbManager.orbs)
        {
            Vector3 vectorFromiOrbToOrb = orb.transform.position - this.transform.position;
            if ((this.name != orb.name && vectorFromiOrbToOrb.magnitude < range) && vectorFromiOrbToOrb.magnitude > 0.6)
            {
                tempOrbList.Add(orb);
            }
        }
        Vector3 sumGravityVector = new Vector3(0, 0, 0);
        foreach (GameObject gravOrb in tempOrbList)
        {
            if(gravOrb.transform.parent==this.transform.parent){
                sumGravityVector += (gravOrb.transform.position - this.transform.position)*2;
            }
            else{
                sumGravityVector += (gravOrb.transform.position - this.transform.position);
            }
        }
        if (tempOrbList.Count > 0)
        {
            sumGravityVector /= tempOrbList.Count;
        }
        this.GetComponent<Rigidbody>().AddForce(sumGravityVector * pull);
        checkGroups(tempOrbList);
    }


    public void checkGroups(List<GameObject> tempOrbList)
    {
        List<GameObject> groupList = new List<GameObject>();
        foreach (GameObject orb in tempOrbList)
        {
            if (((orb.transform.position - this.transform.position).magnitude < 0.7) && orb.transform.parent == null&&orb.name!=this.name)
            {
                groupList.Add(orb);
            }
        }
        if (groupList.Count > 0)
        {
            if (this.transform.parent == null)
            {
                GameObject group = new GameObject("Group of " + this.name);
                foreach (GameObject groupOrb in groupList)
                {
                    groupOrb.transform.parent = group.transform;
                }
                this.transform.parent = group.transform;
            }
            else
            {
                foreach (GameObject groupOrb in groupList)
                {
                    groupOrb.transform.parent = this.transform.parent.transform;
                }
            }
        }
    }
}
