using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserAttack : MonoBehaviour
{
    GameObject[] entities;
    GameObject centerEntity;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /* (entities != null)
            {
                entities = null;
            }
            else
            {
                entities = GameObject.FindGameObjectsWithTag("enemy");
            }*/

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "enemy")
                {

                    Debug.Log("Found " + hit.collider.gameObject.name);
                    centerEntity = hit.collider.gameObject;
                }
            }
        }

        /*if (entities != null)
        {
            for (int i = 0; i < entities.Length - 1; i++)
            {
                Debug.DrawLine(entities[i].transform.position, entities[i + 1].transform.position, Color.red);

            }
        }
        */

        if(centerEntity != null)
        {

        }
        

    }

    void findEntities()
    {

    }
}
