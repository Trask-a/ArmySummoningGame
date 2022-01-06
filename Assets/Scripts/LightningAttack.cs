using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightningAttack : MonoBehaviour
{
    LineRenderer lightning;
    GameObject enemy;
    float lightningTimer;
    float lightningTimerLength = 0.65f;
    public GameObject line;
    GameObject[] entities;
    List<GameObject> totalEnemies = new List<GameObject>();
    public Material lightMat;
    public Button lightningButton;

    // Start is called before the first frame update
    void Start()
    {
        //lightning = GetComponentInChildren<LineRenderer>();    
        lightningTimer = lightningTimerLength;
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            
            // Gets raycast of mouse click on screen
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "enemy")
                {

                    
                    enemy = hit.collider.gameObject;
                    CheckDistance(enemy);
                    
                    if(lightning == null)
                    {
                        lightning = gameObject.AddComponent<LineRenderer>();
                    }
                }               
            }
        }
        */

        if(Input.GetKeyDown(KeyCode.Alpha1) && transform.GetComponent<PlayerMovement>().enemy != null)
        {
            
            LightningActivate();
            lightningButton.GetComponent<Image>().color = Color.gray;
        }
        

        if (lightning != null )
        {
            if(lightningTimer >= 0 && enemy != null)
            {

                int j = 0;
                int k = 1;
                lightning.SetPosition(0, transform.position);
                for (int i = 1; i < totalEnemies.Count+1; i++)
                {

                    lightning.SetPosition(i, totalEnemies[i-1].transform.position);
                   
                }
                lightningTimer -= Time.deltaTime;
                
            }
            else
            {
                foreach (GameObject enem in totalEnemies)
                {
                    if(enem != null)
                    {
                        enem.GetComponent<EnemyController>().TakeDamage(3);
                    }
                }
                //Destroy line compoment
                Destroy(lightning);
                lightningButton.GetComponent<Image>().color = Color.white;
                // reset timer
                lightningTimer = lightningTimerLength;
                
                totalEnemies = new List<GameObject>();
                
            }
            
        }
        
    }

    void CheckDistance(GameObject e)
    {
        entities = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject enem in entities)
        {
            float distance = Vector3.Distance(enem.transform.position, e.transform.position);
            float lightningRadius = 10f;

            if (distance <= lightningRadius)
            {
                if(enem != e)
                {
                    totalEnemies.Add(enem);
                }

            }
        }

    }

    public void LightningActivate()
    {
        if (lightning == null && transform.GetComponent<PlayerMovement>().enemy != null)
        {
            enemy = transform.GetComponent<PlayerMovement>().enemy;
            totalEnemies.Add(enemy);
            CheckDistance(enemy);
            lightning = gameObject.AddComponent<LineRenderer>();
            lightning.material = lightMat;
            lightning.startWidth = 0.4f;
            lightning.endWidth = 0.4f;
            lightning.positionCount = totalEnemies.Count + 1;
            lightningButton.GetComponent<Image>().color = Color.gray;
        }
    }
}

