using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyMenuScript : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (transform.GetChild(0).gameObject.activeSelf == true)
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }

        }
    }
}
