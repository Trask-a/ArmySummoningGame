using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    //public CharacterController controller;
    public float speed = 0;
    public float rotSpeed = 0;
    public float rotTime = 0.1f;
    Vector3 forward, right;

    NavMeshAgent agent;

    public GameObject enemy;

    public float damage = 1f;
    public float health;
    public float maxHealth = 10f;
    public float attackSpeed = 0.5f;

    public int experience = 0;
    public int expToLevelUp = 10;

    float attackCooldown = 0f;
    public int level = 1;

    Animator animController;
    public GameObject damageNumber;
    public GameObject levelUpText;

    public bool healPlayer = false;

    public Button healPlayerButton;

    public Text levelText;
    public Text attackText;
    public Text healthText;
    public Text attackSpeedText;

    public GameObject allyInformation;
    public GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        if(IsClient && IsOwner)
        {
            followPlayer.Instance.FollowPlayer(transform);
        }
        

        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        agent = GetComponent<NavMeshAgent>();
        animController = GetComponent<Animator>();
        health = maxHealth;
        //EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
        allyInformation = GameObject.FindGameObjectWithTag("AllyInformation");
        //TODO FIX
        //levelText.text = "Level: " + level;
        //attackText.text = "Attack: " + damage;
        //healthText.text = "Max Health: " + maxHealth;
        //attackSpeedText.text = "Attack Speed: " + attackSpeed;
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem, int slot)
    {
        if(newItem != null)
        {
            damage += newItem.damageModifier;
            maxHealth += newItem.healthModifier;
            attackSpeed += newItem.attackSpeedModifier;
        }

        if (oldItem != null)
        {
            damage -= oldItem.damageModifier;
            maxHealth -= oldItem.healthModifier;
            if(health >= maxHealth)
            {
                health = maxHealth;
            }
            
            attackSpeed -= oldItem.attackSpeedModifier;
        }

        levelText.text = "Level: " + level;
        attackText.text = "Attack: " + damage;
        healthText.text = "Max Health: " + maxHealth;
        attackSpeedText.text = "Attack Speed: " + attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)
        {
            
            return;
        }

        //if(EventSystem.current.IsPointerOverGameObject())
        //{
          //  return;
        //}

        if (agent.desiredVelocity == new Vector3(0, 0, 0))
        {
            GetComponent<Animator>().SetBool("isRunning", false);
            ServerStopMoveServerRpc();
        }
        // Attack cooldown
        if (attackCooldown >= 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        else
        {
            animController.SetBool("isAttacking", false);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            // stops mouse navigation movement
            agent.velocity = new Vector3(0,0,0);
            agent.isStopped = true;

            // Move function for keyboard movement
            Move();
        }

        if(Input.GetKeyUp(KeyCode.Alpha2))
        {
            healPlayer = true;
            healPlayerButton.GetComponent<Image>().color = Color.gray;
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            ulong clientId = NetworkManager.Singleton.LocalClientId;
            if(!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient))
            {
                return;
            }
            if(!networkClient.PlayerObject.TryGetComponent<ShadowArmyController>(out var shadowArmy))
            {
                return;
            }
            shadowArmy.ServerSummonArmyServerRpc();
            //GetComponent<ShadowArmyController>().ServerSummonArmyServerRpc();
        }

        

        if (Input.GetMouseButtonDown(0))
        {
            // Gets raycast of mouse click on screen
            agent.isStopped = false;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
               
                if (hit.collider.gameObject.tag == "enemy")
                {
                    
                    enemy = hit.collider.gameObject;

                    // Instant rotation towards enemy
                    //transform.LookAt(enemy.transform);
                    
                    //Debug.Log(Vector3.Angle(transform.forward, transform.position - enemy.transform.position));

                    // Needs to have time tied into the updating but not sure how. Rotation over time.
                    //float targetAngle;
                    //float angle;
                    //do
                    //{
                    //     targetAngle = Mathf.Atan2(enemy.transform.position.x, enemy.transform.position.z) * Mathf.Rad2Deg;
                    //     angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotSpeed, rotTime);
                    //     transform.rotation = Quaternion.Euler(0f, angle, 0f);
                    // Debug.Log("TargetAngle = " + targetAngle);
                    // Debug.Log("Angle = " + angle);
                    // } while (Mathf.Round(angle) != Mathf.Round(targetAngle));

                    // Check if player is close enough to enemy then attack
                    if (Vector3.Distance(enemy.transform.position, transform.position) < 5)
                    {
                        Attack(enemy);
                    }
                    else
                    {
                        // Moves the player to raycast hit position
                        //agent.SetDestination(hit.point);
                    }
                }
                else
                {
                    animController.SetBool("isRunning", true);
                    //animController.SetBool("isRunning", true);
                    // Moves the player to raycast hit position
                    ServerMoveServerRpc(hit.point);
                    agent.SetDestination(hit.point);

                    enemy = null;
                }
            }
        }
        if (enemy != null && Vector3.Distance(enemy.transform.position, transform.position) < 5)
        {
            FacePlayer();
        }
        
    }

    [ServerRpc]
    void ServerMoveServerRpc(Vector3 hit)
    {
        ClientMoveClientRpc(hit);
    }

    [ClientRpc]
    void ClientMoveClientRpc(Vector3 hit)
    {
        if (IsOwner) { return; }
        animController.SetBool("isRunning", true);
        agent.SetDestination(hit);
    }

    [ServerRpc]
    void ServerStopMoveServerRpc()
    {
        if(!IsServer)
        {
            return;
        }
        ClientStopMove();
    }

    
    void ClientStopMove()
    {
        //if (IsOwner) { return; }
        animController.SetBool("isRunning", false); 
    }

    // TODO: Fix rotation so that it isn't at an angle
    void Move()
    {
        // Movement from input keys
        Vector3 direction = new Vector3(Input.GetAxisRaw("HorzKey"), 0, Input.GetAxisRaw("VertKey"));
        Vector3 rightMove = right * speed * Time.deltaTime * Input.GetAxis("HorzKey");
        Vector3 upMove = forward * speed * Time.deltaTime * Input.GetAxis("VertKey");

        //Vector3 heading = Vector3.Normalize(rightMove + upMove);
        //transform.forward += heading * rotSpeed * Time.deltaTime;

        // Rotation of player with smoothing
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotSpeed, rotTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
        // Position update
        transform.position += rightMove;
        transform.position += upMove;
    }

    void Attack(GameObject e)
    {
        if(attackCooldown <= 0f)
        {
            animController.SetBool("isAttacking", true);
            // Lowers enemy health by damage and checks if health is less than 0 then rewards exp
            if(e.GetComponent<EnemyController>().getHealth() - damage <= 0)
            {
                AddExperience(e.GetComponent<EnemyController>().expWorth);
            }
            //e.GetComponent<EnemyController>().TakeDamage(damage);
            e.GetComponent<EnemyController>().TakeDamageServerRpc(damage);
            GameObject dn = Instantiate(damageNumber, e.transform.position, new Quaternion(45,45,0,1));
            dn.GetComponent<DamagePopup>().Setup((int)damage);
            
            attackCooldown = 1f / attackSpeed;
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Rotation over time for smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void TakeDamage(float d)
    {
        health -= d;
        if(health <= 0)
        {
            Debug.Log("Player Dead");
        }
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
    }

    public void HealButton()
    {
        healPlayer = true;
        healPlayerButton.GetComponent<Image>().color = Color.gray;
    }

    public void AddExperience(int exp)
    {
        experience += exp;
        if (experience >= expToLevelUp)
        {
            LevelUp();
            int leftOverExp = experience - expToLevelUp;
            experience = leftOverExp;
            expToLevelUp += level * 10;
        }
        
    }

    public void LevelUp()
    {
        level++;
        damage = damage + (level * 2);
        maxHealth = maxHealth + (level * 2);
        attackSpeed = attackSpeed + 0.1f;

        Heal(maxHealth);
        GameObject lut = Instantiate(levelUpText, transform.position, new Quaternion(45, 45, 0, 1));
        lut.GetComponent<DamagePopup>().Setup("Level Up");
    }
}
