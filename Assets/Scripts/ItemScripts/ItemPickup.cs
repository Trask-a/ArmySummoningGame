using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    GameObject player;
    List<Item> commonItems = new List<Item>();
    List<Item> rareItems = new List<Item>();
    List<Item> epicItems = new List<Item>();
    List<Item> legendaryItems = new List<Item>();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SortItems();
        SpawnItem();
    }

    private void OnMouseDown()
    {
        if(Vector3.Distance(player.transform.position, transform.position) < 5f)
        {
            Debug.Log("Picking up: " + item.name);
            if(Inventory.instance.Add(item) == true)
            {
                Destroy(gameObject);
            }
            
        }
        
    }

    void SortItems()
    {   
        string[] guids;
        guids = null;// AssetDatabase.FindAssets("t:Equipment");
        List<string> itemStrings = new List<string>();
        int i = 0;
        Item tempItem;
        foreach (string guid in guids)
        {
            //itemStrings.Add(StringParse(AssetDatabase.GUIDToAssetPath(guid)));
            tempItem = Resources.Load<Item>(itemStrings[i]);
            if ((int)tempItem.rarity == 0)
            {
                commonItems.Add(tempItem);
            }
            else if((int)tempItem.rarity == 1)
            {
                rareItems.Add(tempItem);
            }
            else if ((int)tempItem.rarity == 2)
            {
                epicItems.Add(tempItem);
            }
            else if ((int)tempItem.rarity == 3)
            {
                legendaryItems.Add(tempItem);
            }
            i++;
        }
    }

    string StringParse(string s)
    {
        int slashCount = 0;
        string newString = "";
        foreach (char c in s)
        {
            if (slashCount == 2)
            {
                if(c == '.')
                {
                    return newString;
                }
                newString += c;
            }
            else
            {
                if (c == '/')
                {
                    slashCount++;
                }
            }
            
        }
        return newString;
    }

    void SpawnItem()
    {
        int rand = Random.Range(0, 100);
        // common rarity
        if (rand < 50)
        {
            int commonRand = Random.Range(0, commonItems.Count);
            item = commonItems[commonRand];
        }
        // rare
        else if (rand >= 50 && rand < 85)
        {
            int rareRand = Random.Range(0, rareItems.Count);
            item = rareItems[rareRand];
        }
        // epic
        else if (rand >= 85 && rand < 95)
        {
            int epicRand = Random.Range(0, epicItems.Count);
            item = epicItems[epicRand];
        }
        // Legendary
        else if (rand >= 95 && rand < 100)
        {
            int legendaryRand = Random.Range(0, legendaryItems.Count);
            item = legendaryItems[legendaryRand];
        }
    }
}
