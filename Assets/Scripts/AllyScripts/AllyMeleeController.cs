using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class AllyMeleeController : MonoBehaviour
{
 
    public GameObject enemy;
    public GameObject[] entities;
    float detectionRadius = 5f;
    NavMeshAgent agent;
    public GameObject player;

    float lookRotationSpeed = 5f;

    float attackCooldown = 0f;

    float deathTimer = 1.5f;
    bool foundEnemy = false;
    public GameObject damageNumber;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //player = GameObject.FindGameObjectWithTag("Player");
        findPlayer();
        
    }

    void Update()
    {
        if(player == null)
        {
            findPlayer();
        }
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);
        if (playerDistance >= 10)
        {
            if (agent != null)
            {
                enemy = null;
                if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
                {
                    agent.SetDestination(player.transform.position);
                }
                agent.stoppingDistance = 5;
            }
        }
        else
        {
            // If not targeting an enemy then search for one that is in range.
            if (enemy == null)
            {
                detectionRadius = 5;
                foundEnemy = CheckDistance();
                // If no enemy found then follow player at a farther away position
                if (foundEnemy == false)
                {
                    if (agent != null)
                    {
                        agent.SetDestination(player.transform.position);
                        agent.stoppingDistance = 5;
                    }
                }
            }

            // If player in detection radius then move towards player
            if (enemy != null)
            {
                float enemyDistance = Vector3.Distance(enemy.transform.position, transform.position);
                if (agent != null)
                {
                    if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
                    {
                        agent.SetDestination(enemy.transform.position);
                    }
                    agent.stoppingDistance = 3;
                    detectionRadius = 100;

                    // If player close to enemy then attack and face player
                    if (enemyDistance <= agent.stoppingDistance)
                    {
                        attackCooldown -= Time.deltaTime;
                        Attack(GetComponent<AllyController>().attack.Value);
                        FacePlayer();
                    }
                }


            }
        }
    }

    // Rotates enemy towards target
    void FacePlayer()
    {
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
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
            if(enemy.GetComponent<EnemyController>().getHealth() - damage <= 0)
            {
                GetComponent<AllyController>().AddExperience(enemy.GetComponent<EnemyController>().expWorth);
                player.GetComponent<PlayerMovement>().AddExperience(enemy.GetComponent<EnemyController>().expWorth/2);
            }
            enemy.GetComponent<EnemyController>().TakeDamage(damage);
            GameObject dn = Instantiate(damageNumber, enemy.transform.position, new Quaternion(45, 45, 0, 1));
            dn.GetComponent<DamagePopup>().Setup((int)damage);
            // reset attack cooldown - the greater the attack speed the lower the cooldown
            attackCooldown = 1f / GetComponent<AllyController>().attackSpeed.Value;
        }
    }

    bool CheckDistance()
    {
        entities = GameObject.FindGameObjectsWithTag("enemy");
        float enemyDistance = detectionRadius;
        float distance = 999999;
        foreach (GameObject enem in entities)
        {
            if(enem != null)
            {
                distance = Vector3.Distance(enem.transform.position, transform.position);
            }
            
            float seeRadius = 10f;
            
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
        localClientId = NetworkManager.Singleton.LocalClientId;

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
        {
            return;
        }

        if (networkClient != null)
        {
            if (networkClient.PlayerObject != null)
            {
                if (player == null)
                {
                    player = networkClient.PlayerObject.gameObject;
                }


            }
        }

    }
}
