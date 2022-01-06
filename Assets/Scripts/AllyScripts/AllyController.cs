using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class AllyController : NetworkBehaviour
{
    [SerializeField]
    private NetworkVariable<float> health = new NetworkVariable<float>();
    [SerializeField]
    private NetworkVariable<float> maxHealth = new NetworkVariable<float>(10);
    public NetworkVariable<float> attack = new NetworkVariable<float>(1f);
    public NetworkVariable<float> attackSpeed = new NetworkVariable<float>(1f);
    [SerializeField]
    private NetworkVariable<int> level = new NetworkVariable<int>(0);
    [SerializeField]
    private NetworkVariable<int> experience = new NetworkVariable<int>(0);
    [SerializeField]
    private NetworkVariable<int> experienceToLevelUp = new NetworkVariable<int>(10);
    public GameObject levelUpText;
    public GameObject allyButton;

    private void Start()
    {
        levelUp();
        health = maxHealth;
    }

    public void AddExperience(int exp)
    {
        experience.Value += exp;
        if(experience.Value >= experienceToLevelUp.Value)
        {
            levelUp();
            int leftOverExp = experience.Value - experienceToLevelUp.Value;
            experience.Value = leftOverExp;
            experienceToLevelUp.Value += level.Value * 10;
        }
    }

    public void TakeDamage(float damage)
    {
        health.Value -= damage;

        if (health.Value <= 0)
        {
            transform.tag = "Untagged";
            Destroy(GetComponent<NavMeshAgent>());
            Destroy(gameObject, 1f);
        }


    }

    public void Heal(float amount)
    {
        health.Value += amount;
        if(health.Value >= maxHealth.Value)
        {
            health.Value = maxHealth.Value;
        }
    }

    public float getHealth()
    {
        return health.Value;
    }

    public float getMaxHealth()
    {
        return maxHealth.Value;
    }

    public void setMaxHealth(float h)
    {
        maxHealth.Value = h;
    }

    public void setHealth(float h)
    {
        health.Value = h;
    }

    // Level up ally
    public void levelUp()
    {
        level.Value++;
        if (GetComponent<AllyMeleeController>() != null)
        {
            attack.Value = attack.Value + (level.Value * 4);
            maxHealth.Value = maxHealth.Value + (level.Value * 2);
            attackSpeed.Value = attackSpeed.Value + 0.125f;
        }
        else if (GetComponent<AllyHealerController>() != null)
        {
            attack.Value = attack.Value + (level.Value * 2);
            maxHealth.Value = maxHealth.Value + (level.Value * 2);
            attackSpeed.Value = attackSpeed.Value + 0.115f;
        }
        else if (GetComponent<AllyRangedController>() != null)
        {
            attack.Value = attack.Value + (level.Value * 3);
            maxHealth.Value = maxHealth.Value + (level.Value * 2);
            attackSpeed.Value = attackSpeed.Value + 0.115f;
        }
        else if (GetComponent<AllyTankController>() != null)
        {
            attack.Value = attack.Value + (level.Value * 1);
            maxHealth.Value = maxHealth.Value + (level.Value * 4);
            attackSpeed.Value = attackSpeed.Value + 0.1f;
        }
        
        // Heal ally for half their max health
        Heal(maxHealth.Value * 0.5f);
        GameObject lut = Instantiate(levelUpText, transform.position, new Quaternion(45, 45, 0, 1));
        lut.GetComponent<DamagePopup>().Setup("Level Up");

    }

    public int GetLevel()
    {
        return level.Value;
    }

    public int GetExpToLevel()
    {
        return experienceToLevelUp.Value;
    }

}
