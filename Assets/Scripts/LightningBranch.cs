using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBranch : MonoBehaviour
{

    GameObject[] entities;
    List<GameObject> lightning;
    LineRenderer lr;
    float lightningRadius = 40f;
    GameObject entityInRange;
    public GameObject damageNumber;

    // Start is called before the first frame update
    void Start()
    {
        entities = GameObject.FindGameObjectsWithTag("enemy");
        CheckDistance();
        Destroy(gameObject, 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
        if (entityInRange != null)
        {
            GetComponent<LineRenderer>().SetPosition(0, transform.parent.position);
            GetComponent<LineRenderer>().SetPosition(1, entityInRange.transform.position);
        }
        
    }

    void CheckDistance()
    {
        entities = GameObject.FindGameObjectsWithTag("enemy");

        for (int i = 0; i < entities.Length; i++)
        {
            float distance = Vector3.Distance(entities[i].transform.position, transform.position);


            if (distance <= lightningRadius)
            {
                // Check if entity has already been lined too
                if (entities[i].GetComponent<EnemyController>().GetVBL() == false)
                {
                    entityInRange = entities[i];
                    entities[i].GetComponent<EnemyController>().SetVBL(true);
                    entities[i].GetComponent<EnemyController>().TakeDamage(3);
                    GameObject dn = Instantiate(damageNumber, entities[i].transform.position, new Quaternion(45, 45, 0, 1));
                    dn.GetComponent<DamagePopup>().Setup(3);
                    return;
                }
            }
        }
        
    }
}
