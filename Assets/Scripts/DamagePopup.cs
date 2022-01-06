using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textmesh;
    RectTransform rt;
    float lifetime = 2;

    private void Awake()
    {
        textmesh = transform.GetComponent<TextMeshPro>();
        rt = transform.GetComponent<RectTransform>();
        
    }



    public void Setup(int damage)
    {
        textmesh.SetText(damage.ToString());
        rt.rotation = Quaternion.Euler(45,45,0);
        rt.position = new Vector3(rt.position.x - 1, rt.position.y, rt.position.z - 3);
    }

    public void Setup(string s)
    {
        textmesh.SetText(s);
        rt.rotation = Quaternion.Euler(45, 45, 0);
        rt.position = new Vector3(rt.position.x - 1, rt.position.y, rt.position.z - 3);
    }

    private void Update()
    {
        float YSpeed = 2.5f;
        transform.position += new Vector3(0, YSpeed, 0) * Time.deltaTime;
        if(lifetime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            lifetime -= Time.deltaTime;
        }
        
    }

}
