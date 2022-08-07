using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

// For some reason making this a NetworkBehaviour breaks things and makes the allys constantly spawn an infinite loop
public class AllyButtonScript : NetworkBehaviour
{
    public GameObject ally = null;
    public GameObject stats = null;
    public NetworkVariable<char> allyType = new NetworkVariable<char>();
    public GameObject statsPanel;

    private void Start()
    {
        statsPanel = GameObject.FindGameObjectWithTag("StatsPanel");
        stats = GameObject.FindGameObjectWithTag("Stats");
        transform.GetChild(0).GetComponent<Text>().text = ally.GetComponent<AllyController>().allyName;
    }

    public void FindStatsPanel()
    {
        if(statsPanel == null)
        {
            statsPanel = GameObject.FindGameObjectWithTag("StatsPanel");
        }
    }

    public void RecordAllyStats()
    {
        if(!stats.transform.GetChild(0).gameObject.activeSelf)
        {
            stats.transform.GetChild(0).gameObject.SetActive(true);
            if(statsPanel == null)
            {
                statsPanel = stats.transform.GetChild(0).gameObject;
            }
        }
        
        stats.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Type: " + allyType.Value;
        stats.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "Level: " + ally.GetComponent<AllyController>().GetLevel().ToString();
        stats.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = "Experience To Level: " + ally.GetComponent<AllyController>().GetExpToLevel().ToString();
        stats.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>().text = "Experience: " + ally.GetComponent<AllyController>().GetCurrentExp().ToString();
        stats.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().text = "Health: " + ally.GetComponent<AllyController>().getHealth().ToString();
        stats.transform.GetChild(0).transform.GetChild(5).GetComponent<Text>().text = "Attack: " + ally.GetComponent<AllyController>().attack.Value.ToString();
        stats.transform.GetChild(0).transform.GetChild(6).GetComponent<Text>().text = "Attack Speed: " + ally.GetComponent<AllyController>().attackSpeed.Value.ToString();
    }

    public void DisplayStats()
    {
        for(int i = 0; i < transform.parent.childCount; i++)
        {
            transform.parent.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
        stats.SetActive(true);
        
    }
}
