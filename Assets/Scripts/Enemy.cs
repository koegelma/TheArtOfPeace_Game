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

    private void Start()
    {
        orbManager = OrbManager.instance;
        timeBetweenOrbs = Random.Range(8f, 20f);
        spawnOrbCountdown = timeBetweenOrbs;
        player = GameObject.Find("Main Camera").transform;

        targetNodes = (GameObject)Instantiate(targetNodesPrefab, transform.position, transform.rotation);
        targetNodes.transform.parent = transform;
        targetsScript = targetNodes.GetComponent<PatternTarget>();
    }

    private void Update()
    {
        if (!isUpdating) return;

        transform.LookAt(player);
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
        huhSpawnSound.Play();
    }

    public void ReceiveOrb(GameObject _orb)
    {
        OrbMovement orbScript = _orb.GetComponent<OrbMovement>();
        orbScript.SetTargetArray(targetsScript.targets);
    }
}
