using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject orbPrefab;
    public Transform player;
    public Transform firePosition;
    private OrbManager orbManager;

    //private float countdown = 3f;
    //private float timeBetweenOrbs = 10f;
    private float countdown;//remove - only for testing
    private float timeBetweenOrbs;//remove - only for testing

    private void Start()
    {
        orbManager = OrbManager.instance;
        countdown = Random.Range(4f, 10f);//remove - only for testing
        timeBetweenOrbs = Random.Range(8f, 15f);//remove - only for testing
        //InvokeRepeating("ShootOrb", 0f, 5f);
    }

    private void Update()
    {
        transform.LookAt(player);

        //if (orbManager.HasOrbs) return; //remove - only for testing
        if (Pattern1.isCountdown && Pattern1.patternTargetsCountdown > 2) return;

        if (countdown <= 0f)
        {
            ShootOrb();
            countdown = timeBetweenOrbs;
            return;
        }
        countdown -= Time.deltaTime;
    }

    private void ShootOrb()
    {
        Instantiate(orbPrefab, firePosition.position, transform.rotation);
    }

    public void ReceiveOrb(GameObject orb)
    {
        OrbMovement orbScript = orb.GetComponent<OrbMovement>();
        // PatternNodes instantiaten
        // TargetArray dem OrbMovement übergeben
        // OrbMovement checkt im GetNextTarget() ob Enemy und ändert Target zu player
    }
}
