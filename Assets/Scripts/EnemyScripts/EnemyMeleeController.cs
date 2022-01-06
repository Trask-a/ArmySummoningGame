using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class EnemyMeleeController : NetworkBehaviour
{
    public GameObject player;
    float detectionRadius = 5f;
    NavMeshAgent agent;

    float lookRotationSpeed = 5f;

    float attackCooldown = 0f;
    float attackSpeed = 1f;

    public GameObject enemy;
    public GameObject[] entities;
    bool foundEnemy = false;
    public GameObject damageNumber;
    public NetworkVariable<Vector3> roamPosition = new NetworkVariable<Vector3>();
    float roamTimer;
    float roamTime = 3;
    float idleTimer;
    float idleTime = 3;
    AIState state = AIState.roam;
    enum AIState {idle, roam}

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        roamTimer = roamTime;
    }

    void Update()
    {
        if (!IsOwner)
        {

            return;
        }
        // If not targeting an enemy then search for one that is in range.
        if (enemy == null)
        {
            detectionRadius = 5;
            foundEnemy = CheckDistance();
            // If no enemy found then roam
            if (foundEnemy == false)
            {
                if (agent != null)
                {
                    switch (state)
                    {
                        case AIState.idle:
                            idleTimer -= Time.deltaTime;
                            break;
                        case AIState.roam:
                            roamTimer -= Time.deltaTime;
                            break;
                    }

                    if (roamTimer <= 0)
                    {
                        Vector3 temp = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
                        roamPosition.Value = transform.position + temp;
                        agent.SetDestination(roamPosition.Value);
                        roamTimer = roamTime;
                        state = AIState.idle;
                    }
                    
                    if(idleTimer <= 0)
                    {
                        idleTimer = Random.Range(1, 4);
                        state = AIState.roam;
                    }
                }
            }
        }

        // If enemy in detection radius then move towards enemy
        if (enemy != null)
        {
            float enemyDistance = Vector3.Distance(enemy.transform.position, transform.position);
            if (agent != null)
            {
                if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
                {
                    agent.SetDestination(enemy.transform.position);
                }
                detectionRadius = 100;

                // If player close to enemy then attack and face player
                if (enemyDistance <= agent.stoppingDistance)
                {
                    attackCooldown -= Time.deltaTime;
                    Attack(GetComponent<EnemyController>().attack);
                    FacePlayer();
                }
            }


        }
    }

    // Rotates enemy towards target
    void FacePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Rotation over time for smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    // Damages player with amount of damage
    void Attack(float damage)
    {
        // Check if attack cooldown is 0
        if (attackCooldown <= 0f)
        {

            if (enemy.tag == "Player")
            { 
                enemy.GetComponent<PlayerMovement>().TakeDamage(damage);
            }

            if (enemy.tag == "Ally")
            {
                enemy.GetComponent<AllyController>().TakeDamage(damage);
            }
            GameObject dn = Instantiate(damageNumber, enemy.transform.position, new Quaternion(45, 45, 0, 1));
            dn.GetComponent<DamagePopup>().Setup((int)damage);
            // reset attack cooldown - the greater the attack speed the lower the cooldown
            attackCooldown = 1f / GetComponent<EnemyController>().attackSpeed;
        }

    }

    bool CheckDistance()
    {
        findPlayer();
        float enemyDistance = detectionRadius;
        entities = GameObject.FindGameObjectsWithTag("Ally");
        float distance = 999999;
        foreach (GameObject enem in entities)
        {
            if (enem != null)
            {
                distance = Vector3.Distance(enem.transform.position, transform.position);
            }
            float seeRadius = 15f;

            if (enemy != null)
            {
                enemyDistance = Vector3.Distance(enemy.transform.position, transform.position);
            }


            if (distance <= seeRadius)
            {
                if (enemy == null)
                {
                    enemy = enem;
                }
                else if (distance < enemyDistance)
                {
                    enemy = enem;
                }

            }
        }

        float playerDistance = Vector3.Distance(player.transform.position, transform.position);
        if (playerDistance < enemyDistance)
        {
            enemy = player;
        }

        if (enemy != null)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void findPlayer()
    {
        ulong localClientId = 0;
        for(int i = 0; i < NetworkManager.ConnectedClientsList.Count; i++)
        {
            localClientId = NetworkManager.ConnectedClientsList[i].ClientId;
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
            { 
                return;
            }

            if (networkClient != null)
            {
                if (networkClient.PlayerObject != null)
                {
                    if(player == null)
                    {
                        player = networkClient.PlayerObject.gameObject;
                    }
                    else
                    {
                        float playerDistance = Vector3.Distance(player.transform.position, transform.position);
                        float otherPlayerDistance = Vector3.Distance(networkClient.PlayerObject.gameObject.transform.position, transform.position);
                        if (otherPlayerDistance < playerDistance)
                        {
                            player = networkClient.PlayerObject.gameObject;
                        }
                    }
                    
                }
            }
        }
        /*
        if(NetworkManager.ConnectedClients.Count == 0)
        {

            return;
        }
        else
        {
            localClientId = NetworkManager.ConnectedClientsList[0].ClientId;
        }

       // if (localClientId != NetworkManager.Singleton.LocalClientId)
       // {
            //localClientId = NetworkManager.Singleton.LocalClientId;
       // }
        

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
        {

            return;
        }


        if (networkClient != null)
        {
            if (networkClient.PlayerObject != null)
            {
                player = networkClient.PlayerObject.gameObject;
                

            }
        }
        */
    }
}
