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
        statsPanel.transform.GetChild(0).GetComponent<Text>().text = "Type: " + allyType.Value;
        statsPanel.transform.GetChild(1).GetComponent<Text>().text = "Level: " + ally.GetComponent<AllyController>().GetLevel().ToString();
        statsPanel.transform.GetChild(2).GetComponent<Text>().text = "Experience To Level: " + ally.GetComponent<AllyController>().GetExpToLevel().ToString();
        statsPanel.transform.GetChild(3).GetComponent<Text>().text = "Health: " + ally.GetComponent<AllyController>().getMaxHealth().ToString();
        statsPanel.transform.GetChild(4).GetComponent<Text>().text = "Attack: " + ally.GetComponent<AllyController>().attack.Value.ToString();
        statsPanel.transform.GetChild(5).GetComponent<Text>().text = "Attack Speed: " + ally.GetComponent<AllyController>().attackSpeed.Value.ToString();
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
