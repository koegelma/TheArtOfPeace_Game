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
    private GameObject recievedOrb;
    private float destroyCountdown;
    public GameObject currentCooldownBar;
    public bool isUpdating = true;
    public AudioSource huhSpawnSound;
    //private float timeToDestroy = 10f;
    //private bool isDestroyCountdown = false;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        orbManager = OrbManager.instance;
        timeBetweenOrbs = Random.Range(8f, 20f);//remove - only for testing
        spawnOrbCountdown = timeBetweenOrbs;//remove - only for testing
        //destroyCountdown = timeToDestroy;
        player = GameObject.Find("Main Camera").transform;
        recievedOrb = null;
        //InvokeRepeating("ShootOrb", 0f, 5f);
    }

    private void Update()
    {
        //if (orbManager.HasOrbs) return;
        if (!isUpdating) return;

        Vector3 targetPostition = new Vector3(player.position.x, transform.position.y, player.position.z) ;
        transform.LookAt(targetPostition);
        currentCooldownBar.transform.localScale = new Vector3(spawnOrbCountdown / timeBetweenOrbs, currentCooldownBar.transform.localScale.y, currentCooldownBar.transform.localScale.z);

        if (recievedOrb != null) CheckRecievedOrbStatus();

        //if (isDestroyCountdown) DestroyCountdown();

        //if (orbManager.HasOrbs) return; //remove - only for testing

        //if (Pattern.isCountdown && Pattern.patternTargetsCountdown > 2) return; // adjust for multiple patterns

        if (spawnOrbCountdown <= 0f)
        {
            ShootOrb();
            spawnOrbCountdown = GetTimeBetweenOrbs();
            return;
        }
        spawnOrbCountdown -= Time.deltaTime / (OrbManager.instance.OrbsInGame + 1);
    }

    private float GetTimeBetweenOrbs()
    {
        timeBetweenOrbs = Random.Range(10f, 20f);
        return timeBetweenOrbs;
    }

    private void ShootOrb()
    {
        Instantiate(orbPrefab, firePosition.position, transform.rotation);
        animator.SetTrigger("Punch Trigger");
        huhSpawnSound.Play();
    }

    public IEnumerator ReceiveOrb(GameObject _orb)
    {
        Debug.Log(gameObject.name + " recieved " + _orb.name);
        recievedOrb = _orb;
        OrbMovement orbScript = _orb.GetComponent<OrbMovement>();
        targetNodes = (GameObject)Instantiate(targetNodesPrefab, transform.position, transform.rotation);
        targetNodes.transform.parent = transform;
        //isDestroyCountdown = true;
        PatternTarget targetsScript = targetNodes.GetComponent<PatternTarget>();
        //targetsScript.SetEnemyTransform(this.transform);
        Debug.Log(gameObject.name + " waiting for " + _orb.name);
        yield return new WaitUntil(() => targetsScript.isInitialized);
        Debug.Log("targetscript initialized for " + _orb.name);
        orbScript.SetTargetArray(targetsScript.targets);
    }

    private void CheckRecievedOrbStatus()
    {
        OrbMovement orbScript = recievedOrb.GetComponent<OrbMovement>();
        if (orbScript.isFinalEnemyTargetPassed)
        {
            orbScript.isFinalEnemyTargetPassed = false;
            DestroyTargetNodes();
        }
    }

    private void DestroyCountdown()
    {
        if (destroyCountdown <= 0)
        {
            DestroyTargetNodes();
            return;
        }
        destroyCountdown -= Time.deltaTime;
    }

    private void DestroyTargetNodes()
    {
        //isDestroyCountdown = false;
        Destroy(targetNodes);
        //Debug.Log("Enemy Target Nodes destroyed");
        //destroyCountdown = timeToDestroy;
        recievedOrb = null;
    }
}
