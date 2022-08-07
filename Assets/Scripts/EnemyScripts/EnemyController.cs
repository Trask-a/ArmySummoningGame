using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class EnemyController : NetworkBehaviour
{
    [SerializeField]
    private NetworkVariable<float> health = new NetworkVariable<float>();
    [SerializeField]
    private float maxHealth = 10;
    public float attack = 1f;
    public float attackSpeed = 1f;
    [SerializeField]
    private int level = 0;
    public int expWorth = 5;

    public GameObject soul;
    bool dead = false;
    public GameObject loot;

    // vbl = visited by lightning
    private bool vbl = false;
    public GameObject spawner;

    GameObject s = null;

    public UnitUIControler unitUI;

    public Dictionary<GameObject, float> damageTaken = new Dictionary<GameObject, float>();

    // Is this enemy aggroed by a tank
    bool isTankAggroed = false;

    private void Start()
    {
        // Uncomment for multiplayer
       // if(!IsHost)
        //{
          //  return;
        //}
        levelUp();
        health.Value = maxHealth;
        unitUI.SetHealth(health.Value, maxHealth);
        unitUI.SetAllyName("NON BEAK Gamer " + Random.Range(1, 1000).ToString());
    }

    public void TakeDamage(float damage, GameObject enemy)
    {
        health.Value -= damage;
        unitUI.SetHealth(health.Value, maxHealth);
        // Keep track of total damage from all enemies
        if(damageTaken.ContainsKey(enemy))
        {
            damageTaken[enemy] += damage;
        }
        else
        {
            damageTaken.Add(enemy, damage);
        }

        if (!isTankAggroed)
        {
            GameObject enemyToAggro = null;
            float highestAmountOfDamage = 0;
            foreach (KeyValuePair<GameObject, float> enemyDamage in damageTaken)
            {
                if (enemyDamage.Value > highestAmountOfDamage)
                {
                    highestAmountOfDamage = enemyDamage.Value;
                    enemyToAggro = enemyDamage.Key;
                }
            }

            if (TryGetComponent<EnemyMeleeController>(out EnemyMeleeController meleeControler))
            {
                meleeControler.enemy = enemyToAggro;
            }
        }

        if (health.Value <= 0 && dead == false)
        {
            // Uncomment for multiplayer and recomment out Die();
            //if (!IsOwner) { return; }
            //ServerDieServerRpc();
            Die();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        if (!IsOwner) { return; }
        health.Value -= damage;
        if (health.Value <= 0 && dead == false)
        {
            //ServerDieServerRpc();
            DieClientRpc();
            //Die();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerDieServerRpc()
    {

        DieClientRpc();
    }

    [ClientRpc]
    public void DieClientRpc()
    {
        Die();
    }

    public bool GetVBL()
    {
        return vbl;
    }

    public void SetVBL(bool l)
    {
        vbl = l;
    }

    public void Heal(float amount)
    {
        health.Value += amount;
        if (health.Value >= maxHealth)
        {
            health.Value = maxHealth;
        }
    }

    public float getHealth()
    {
        return health.Value;
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    public void levelUp()
    {
        level++;
        if (GetComponent<EnemyMeleeController>() != null)
        {
            attack = attack + (level * 4);
            maxHealth = maxHealth + (level * 2);
            attackSpeed = attackSpeed + 0.125f;
        }
        else if (GetComponent<EnemyHealerController>() != null)
        {
            attack = attack + (level * 2);
            maxHealth = maxHealth + (level * 2);
            attackSpeed = attackSpeed + 0.115f;
        }
        else if (GetComponent<EnemyRangedController>() != null)
        {
            attack = attack + (level * 3);
            maxHealth = maxHealth + (level * 2);
            attackSpeed = attackSpeed + 0.115f;
        }
        else if (GetComponent<EnemyTankContoller>() != null)
        {
            attack = attack + (level * 1);
            maxHealth = maxHealth + (level * 4);
            attackSpeed = attackSpeed + 0.1f;
        }

    }

    void Die()
    {
        // Uncomment for multiplayer
        //if (!IsHost)
        //{
          //  return;
        //}
        dead = true;
        //THIS LINE WE HAVE TO GET RID OF but also not sure how shit will work..... 
        s = Instantiate(soul, transform.position, transform.rotation);
        //Uncomment for multiplayer
        //s.GetComponent<NetworkObject>().Spawn();
        // I think i need to send all this data to the serverRpc....
        s.GetComponent<Soul>().attack.Value = attack;
        s.GetComponent<Soul>().attackSpeed.Value = attackSpeed;
        s.GetComponent<Soul>().health.Value = maxHealth;
        s.GetComponent<Soul>().level.Value = level;
        if (GetComponent<EnemyMeleeController>() != null)
        {
            s.GetComponent<Soul>().type.Value = 'm';
        }
        else if (GetComponent<EnemyHealerController>() != null)
        {
            s.GetComponent<Soul>().type.Value = 'h';
        }
        else if (GetComponent<EnemyRangedController>() != null)
        {
            s.GetComponent<Soul>().type.Value = 'r';
        }
        else if (GetComponent<EnemyTankContoller>() != null)
        {
            s.GetComponent<Soul>().type.Value = 't';
        }

        
        //SpawnSoulServerRpc();

        // UNCOMMENT WHEN LOOT WORKS AGAIN
        //DropLoot();
        if (spawner != null)
        {
            spawner.GetComponent<Spawner>().enemiesAlive.Remove(gameObject);
        }
        transform.tag = "Untagged";
        Destroy(GetComponent<NavMeshAgent>());
        Destroy(gameObject, 1f);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnSoulServerRpc()
    {

        if(IsHost)
        {
            s.GetComponent<NetworkObject>().Spawn();
        }
        

    }

    void DropLoot()
    {
        Vector3 spawnPos = new Vector3(transform.position.x,(transform.position.y - transform.localScale.y / 2), transform.position.z);
        Instantiate(loot, spawnPos, transform.rotation);
    }

    public void TankAggroed(float aggroTime)
    {
        isTankAggroed = true;
        StartCoroutine(Aggroed(aggroTime));
    }

    public IEnumerator Aggroed(float aggroTime)
    {
        float disAmount = 0;
        while (disAmount <= aggroTime)
        {
            yield return new WaitForSeconds(0.1f);
            disAmount += 0.1f;
        }
        isTankAggroed = false;
    }
}
