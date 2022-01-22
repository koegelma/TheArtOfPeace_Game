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
    private PatternTarget targetsScript;
    public GameObject currentCooldownBar;
    public bool isUpdating = true;
    public AudioSource huhSpawnSound;
    Animator animator;
    

    private void Start()
    {
        animator = GetComponent<Animator>();
        orbManager = OrbManager.instance;
        timeBetweenOrbs = Random.Range(8f, 20f);
        spawnOrbCountdown = timeBetweenOrbs;
        player = GameObject.Find("Main Camera").transform;

        //Vector3 targetNodesPosition = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
        targetNodes = (GameObject)Instantiate(targetNodesPrefab, transform.position, transform.rotation);
        targetNodes.transform.parent = transform;
        targetsScript = targetNodes.GetComponent<PatternTarget>();
    }

    private void Update()
    {
        if (!isUpdating) return;

        Vector3 targetPostition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPostition);
        int stateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        if(stateHash == Animator.StringToHash("Base Layer.Mma Idle") || stateHash == Animator.StringToHash("Base Layer.Fight Idle")){
            transform.RotateAround(transform.position, Vector3.up, 20);
        }
        transform.GetChild(0).transform.LookAt(targetPostition);
        currentCooldownBar.transform.localScale = new Vector3(spawnOrbCountdown / timeBetweenOrbs, currentCooldownBar.transform.localScale.y, currentCooldownBar.transform.localScale.z);

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
        animator.SetTrigger("Attack Trigger");
        huhSpawnSound.Play();
    }

    public void ReceiveOrb(GameObject _orb)
    {
        OrbMovement orbScript = _orb.GetComponent<OrbMovement>();
        orbScript.SetTargetArray(targetsScript.targets);
    }
}
