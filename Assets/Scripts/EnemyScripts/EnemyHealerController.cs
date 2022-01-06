using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealerController : MonoBehaviour
{
    public GameObject enemy;
    public GameObject[] entities;
    float detectionRadius = 5f;
    UnityEngine.AI.NavMeshAgent agent;
    public GameObject player;

    float lookRotationSpeed = 5f;

    float attackCooldown = 0f;
    // Higher = faster, Lower = slower
    float attackSpeed = 0.5f;

    float deathTimer = 1.5f;
    bool foundEnemy = false;

    public GameObject damageNumber;
    public float healAmount = 4;
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
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        roamTimer = roamTime;
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
                    agent.stoppingDistance = 7;
                    detectionRadius = 100;

                    // If player close to enemy then attack and face player
                    if (enemyDistance <= agent.stoppingDistance)
                    {
                        attackCooldown -= Time.deltaTime;
                        if (enemy != transform.gameObject)
                        {
                            FacePlayer();
                        }
                        Heal(GetComponent<EnemyController>().attack);
                        enemy = null;
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

    void Heal(float h)
    {
        // Check if attack cooldown is 0
        if (attackCooldown <= 0f)
        {
            // Spawn Projectile
            enemy.GetComponent<EnemyController>().Heal(h);
            GameObject dn = Instantiate(damageNumber, enemy.transform.position, new Quaternion(45, 45, 0, 1));
            dn.GetComponent<DamagePopup>().Setup((int)h);
            // reset attack cooldown - the greater the attack speed the lower the cooldown
            attackCooldown = 1f / GetComponent<EnemyController>().attackSpeed;
        }
    }

    bool CheckDistance()
    {
        // Find all ally's 
        // if focus player is true then heal player
        // else find which tank ally is lowest and heal them first
        // else find which melee are lowest and heal them first
        // else find which range is lowest and heal them first
        // else check if self health is low and then heal
        // else don't heal

        entities = GameObject.FindGameObjectsWithTag("enemy");
        float enemyDistance = detectionRadius;
        float seeRadius = 15f;
        GameObject temp = null;
        string tempType = "";
        string enemyType = "";
        foreach (GameObject enem in entities)
        {
            if (enem.GetComponent<EnemyTankContoller>() != null && CheckHealth(enem) == true)
            {

                temp = enem;
                tempType = "tank";

            }
            else if (enem.GetComponent<EnemyMeleeController>() != null && CheckHealth(enem) == true)
            {

                temp = enem;
                tempType = "melee";

            }
            else if (enem.GetComponent<EnemyRangedController>() != null && CheckHealth(enem) == true)
            {

                temp = enem;
                tempType = "ranged";

            }
            else
            {
                if (CheckHealth(transform.gameObject) == true)
                {
                    temp = transform.gameObject;
                    tempType = "self";
                }
            }


            if (temp != null)
            {
                float distance = Vector3.Distance(temp.transform.position, transform.position);
                if (distance <= seeRadius)
                {
                    if (enemy != null)
                    {
                        if (tempType == enemyType)
                        {
                            enemyDistance = Vector3.Distance(enemy.transform.position, transform.position);
                            if (distance < enemyDistance)
                            {
                                enemy = temp;
                                enemyType = tempType;
                            }
                        }
                        else if (tempType == "tank" && enemyType == "melee" || tempType == "tank" && enemyType == "ranged" || tempType == "tank" && enemyType == "self" || tempType == "melee" && enemyType == "ranged" || tempType == "melee" && enemyType == "self" || tempType == "ranged" && enemyType == "self")
                        {
                            enemy = temp;
                            enemyType = tempType;
                        }

                    }
                    else
                    {
                        enemy = temp;
                        enemyType = tempType;
                    }

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

    // Check if entity's health is below x number
    bool CheckHealth(GameObject e)
    {
        if (e.GetComponent<EnemyController>().getHealth() <= e.GetComponent<EnemyController>().getMaxHealth() / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
