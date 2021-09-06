using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject orbPrefab;
    public Transform player;
    public Transform firePosition;

    private float countdown = 3f;
    private float timeBetweenOrbs = 10f;

    private void Start()
    {
        //InvokeRepeating("ShootOrb", 0f, 5f);
    }

    private void Update()
    {
        transform.LookAt(player);

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
}
