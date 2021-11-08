using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject orbPrefab;
    private Transform player;
    public Transform firePosition;
    private OrbManager orbManager;
    private float spawnOrbCountdown;
    private float timeBetweenOrbs;
    public GameObject targetNodesPrefab;
    private GameObject targetNodes;
    private float destroyCountdown;
    private float timeToDestroy = 10f;
    private bool isDestroyCountdown = false;

    private void Start()
    {
        orbManager = OrbManager.instance;
        spawnOrbCountdown = Random.Range(4f, 10f);//remove - only for testing
        timeBetweenOrbs = Random.Range(8f, 15f);//remove - only for testing
        destroyCountdown = timeToDestroy;
        player = GameObject.Find("Main Camera").transform;
        //InvokeRepeating("ShootOrb", 0f, 5f);
    }

    private void Update()
    {
        transform.LookAt(player);

        if (isDestroyCountdown) DestroyCountdown();


        if (orbManager.HasOrbs) return; //remove - only for testing
        if (Pattern1.isCountdown && Pattern1.patternTargetsCountdown > 2) return;

        if (spawnOrbCountdown <= 0f)
        {
            ShootOrb();
            spawnOrbCountdown = timeBetweenOrbs;
            return;
        }
        spawnOrbCountdown -= Time.deltaTime;
    }

    private void ShootOrb()
    {
        Instantiate(orbPrefab, firePosition.position, transform.rotation);
    }

    public IEnumerator ReceiveOrb(GameObject orb)
    {
        OrbMovement orbScript = orb.GetComponent<OrbMovement>();
        targetNodes = (GameObject)Instantiate(targetNodesPrefab, transform.position, transform.rotation);
        targetNodes.transform.parent = transform;
        isDestroyCountdown = true;
        PatternTarget targetsScript = targetNodes.GetComponent<PatternTarget>();
        //targetsScript.SetEnemyTransform(this.transform);
        yield return new WaitUntil(() => targetsScript.isInitialized);
        orbScript.SetTargetArray(targetsScript.targets);
    }

    private void DestroyCountdown()
    {
        if (destroyCountdown <= 0)
        {
            isDestroyCountdown = false;
            Destroy(targetNodes);
            Debug.Log("Enemy Target Nodes destroyed");
            destroyCountdown = timeToDestroy;
            return;
        }
        destroyCountdown -= Time.deltaTime;
    }

    /* targetsGameObject = (GameObject)Instantiate(patternTargetPrefab, Vector3.zero, transform.rotation);
       isCountdown = true;
       PatternTarget targetsScript = targetsGameObject.GetComponent<PatternTarget>();
       yield return new WaitUntil(() => targetsScript.isInitialized);

       targets = targetsScript.targets; */

}
