using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Spawner : NetworkBehaviour
{
    public int radius = 10;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public GameObject enemy;
    public int numberOfEnemies = 3;
    public List<GameObject> enemiesAlive = new List<GameObject>();
    public float respawnTimer = 4f;
    float respawnTime;

    private void Start()
    {
        respawnTime = respawnTimer;
    }

    private void Update()
    {
        if(enemiesAlive.Count == 0)
        {
            respawnTime -= Time.deltaTime;
            if(respawnTime <= 0)
            {
                SpawnEnemies();
                respawnTime = respawnTimer;
            }
            
        }
    }

    void SpawnEnemies()
    {
        //if(!IsHost)
        //{
          //  return;
       // }
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector2 temp = Random.insideUnitCircle * radius;
            Vector3 spawnPos = new Vector3(transform.position.x + temp.x, transform.position.y, transform.position.z + temp.y);
            
            GameObject e = Instantiate(enemy, spawnPos, transform.rotation);
            e.GetComponent<EnemyController>().spawner = gameObject;
            e.GetComponent<NetworkObject>().Spawn();
            enemiesAlive.Add(e);
        }
    }
}
