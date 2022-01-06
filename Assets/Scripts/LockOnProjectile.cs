using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnProjectile : MonoBehaviour
{
    public float speed = 50;
    float lifetime = 5f;
    Vector3 enemyForward;
    Vector3 enemyLocation;
    float timeElapsed = 0f;
    float lerpDuration = 3f;
    Vector3 direction;
    GameObject entity;
    public float damage = 5;
    // player damage number
    public GameObject PDN;
    // enemy damage number
    public GameObject EDN;
    // ally damage number
    public GameObject ADN;

    // Start is called before the first frame update
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        if (entity != null)
        {
            float distanceThisFrame = speed * Time.deltaTime;
            direction = entity.transform.position - transform.position;
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
            Destroy(gameObject, 10f);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (entity != null)
        {
            if (entity.tag == "Player")
            {
                if (other.tag == "Player")
                {
                    entity.GetComponent<PlayerMovement>().TakeDamage(damage);
                    GameObject dn = Instantiate(EDN, entity.transform.position, new Quaternion(45, 45, 0, 1));
                    dn.GetComponent<DamagePopup>().Setup((int)damage);
                    Destroy(gameObject);
                }
            }
            else if (entity.tag == "Ally")
            {
                if (other.tag == "Ally")
                {
                    entity.GetComponent<AllyController>().TakeDamage(damage);
                    GameObject dn = Instantiate(EDN, entity.transform.position, new Quaternion(45, 45, 0, 1));
                    dn.GetComponent<DamagePopup>().Setup((int)damage);
                    Destroy(gameObject);
                }
            }
            else if (entity.tag == "enemy")
            {
                if (other.tag == "enemy")
                {
                    entity.GetComponent<EnemyController>().TakeDamage(damage);
                    GameObject dn = Instantiate(ADN, entity.transform.position, new Quaternion(45, 45, 0, 1));
                    dn.GetComponent<DamagePopup>().Setup((int)damage);
                    Destroy(gameObject);
                }
            }
        }
    }

    public void Setup(GameObject e, float d)
    {
        damage = d;
        entity = e;
        enemyLocation = entity.transform.position;
        enemyForward = entity.transform.forward;
    }
}
