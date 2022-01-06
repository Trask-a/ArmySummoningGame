using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AllyHealerController : MonoBehaviour
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

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
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
            if (player.GetComponent<PlayerMovement>().healPlayer == true)
            {
                enemy = player;
            }
            
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
                        if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
                        {
                            agent.SetDestination(player.transform.position);
                        }
                        agent.stoppingDistance = 5;
                    }
                }
            }
            // If enemy in detection radius then move towards enemy
            else if (enemy != null)
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
                        if (player.GetComponent<PlayerMovement>().healPlayer == true)
                        {
                            HealPlayer(GetComponent<AllyController>().attack.Value);
                            //if(player.GetComponent<PlayerMovement>().health >= player.GetComponent<PlayerMovement>().maxHealth)
                            //{
                            //   player.GetComponent<PlayerMovement>().healPlayer = false;
                            //   enemy = null;
                            //}
                            enemy = null;
                        }
                        else
                        {
                            Heal(GetComponent<AllyController>().attack.Value);
                            enemy = null;
                        }
                        
                        
                        
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

    void Heal(float h)
    {
        // Check if attack cooldown is 0
        if (attackCooldown <= 0f)
        {
            // Spawn Projectile
            if (enemy.GetComponent<AllyController>() != null)
            {                
                enemy.GetComponent<AllyController>().Heal(h);
                GameObject dn = Instantiate(damageNumber, enemy.transform.position, new Quaternion(45, 45, 0, 1));
                dn.GetComponent<DamagePopup>().Setup((int)h);
                GetComponent<AllyController>().AddExperience(1);
            }
            
            // reset attack cooldown - the greater the attack speed the lower the cooldown
            attackCooldown = 1f / GetComponent<AllyController>().attackSpeed.Value;
        }
    }

    void HealPlayer(float h)
    {
        // Check if attack cooldown is 0
        if (attackCooldown <= 0f)
        {
            player.GetComponent<PlayerMovement>().Heal(h);
            player.GetComponent<PlayerMovement>().healPlayer = false;
            player.GetComponent<PlayerMovement>().healPlayerButton.GetComponent<Image>().color = Color.white;
            GameObject dn = Instantiate(damageNumber, player.transform.position, new Quaternion(45, 45, 0, 1));
            dn.GetComponent<DamagePopup>().Setup((int)h);
            GetComponent<AllyController>().AddExperience(1);
            // reset attack cooldown - the greater the attack speed the lower the cooldown
            attackCooldown = 1f / GetComponent<AllyController>().attackSpeed.Value;
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

        entities = GameObject.FindGameObjectsWithTag("Ally");
        float enemyDistance = detectionRadius;
        float seeRadius = 15f;
        GameObject temp = null;
        string tempType = "";
        string enemyType = "";
        foreach (GameObject enem in entities)
        {
            if (enem.GetComponent<AllyTankController>() != null && CheckHealth(enem) == true)
            {

                temp = enem;
                tempType = "tank";

            }
            else if (enem.GetComponent<AllyMeleeController>() != null && CheckHealth(enem) == true)
            {
                
                temp = enem;
                tempType = "melee";
               
            }
            else if (enem.GetComponent<AllyRangedController>() != null && CheckHealth(enem) == true)
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
                        if(tempType == enemyType)
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
        if (e.GetComponent<AllyController>().getHealth() <= e.GetComponent<AllyController>().getMaxHealth() / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
