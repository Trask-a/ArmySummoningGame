using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTankContoller : MonoBehaviour
{
    public GameObject enemy;
    public GameObject[] entities;
    float detectionRadius = 5f;
    NavMeshAgent agent;
    public GameObject player;

    float lookRotationSpeed = 5f;

    float attackCooldown = 0f;
    float attackSpeed = 1f;

    float deathTimer = 1.5f;
    bool foundEnemy = false;
    public GameObject damageNumber;

    AIState state = AIState.roam;
    float idleTimer;
    float idleTime = 3;
    float roamTimer;
    float roamTime = 3;
    Vector3 roamPosition;

    public enum AIState
    {
        idle,
        roam
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        
        
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
                            Vector3 temp = new Vector3(Random.Range(-10, 11), 0, Random.Range(-10, 11));
                            roamPosition = transform.position + temp;
                            agent.SetDestination(roamPosition);
                            roamTimer = roamTime;
                            state = AIState.idle;
                        }

                        if (idleTimer <= 0)
                        {
                            idleTimer = Random.Range(1, 4);
                            state = AIState.roam;
                        }
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
                        int rand = Random.Range(0, 2);
                        if(enemy == player || enemy.GetComponent<AllyHealerController>() != null || enemy.GetComponent<AllyTankController>() != null)
                        {
                            Attack(GetComponent<EnemyController>().attack);
                        }
                        else if (rand == 0)
                        {
                            Attack(GetComponent<EnemyController>().attack);
                        }
                        else if (rand == 1)
                        {
                            Aggro();
                        }

                        FacePlayer();
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

    void Aggro()
    {
        // Check if attack cooldown is 0
        if (attackCooldown <= 0f)
        {
            if (enemy.GetComponent<AllyMeleeController>() != null)
            {
                enemy.GetComponent<AllyMeleeController>().enemy = transform.gameObject;
            }
            else if (enemy.GetComponent<AllyRangedController>() != null)
            {
                enemy.GetComponent<AllyRangedController>().enemy = transform.gameObject;
            }
            GameObject dn = Instantiate(damageNumber, enemy.transform.position, new Quaternion(45, 45, 0, 1));
            dn.GetComponent<DamagePopup>().Setup("Aggro");
            // reset attack cooldown - the greater the attack speed the lower the cooldown
            attackCooldown = 1f / GetComponent<EnemyController>().attackSpeed;
        }
    }

    bool CheckDistance()
    {
        entities = GameObject.FindGameObjectsWithTag("Ally");
        float enemyDistance = detectionRadius;
        float distance = 999999;
        foreach (GameObject enem in entities)
        {
            if (enem != null)
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
}
