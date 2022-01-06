using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemStatsUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject stats;
    public GameObject slot; 
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ShowStats());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        stats.SetActive(false);
    }

    public IEnumerator ShowStats()
    {

        stats.SetActive(true);
        stats.transform.GetChild(0).GetComponent<Text>().text = slot.GetComponent<InventorySlot>().item.name;
        stats.transform.GetChild(1).GetComponent<Text>().text = slot.GetComponent<InventorySlot>().item.rarity.ToString();
        stats.transform.GetChild(2).GetComponent<Text>().text = slot.GetComponent<InventorySlot>().item.description;
        Equipment tempEquipment = slot.GetComponent<InventorySlot>().item as Equipment;
        if (tempEquipment != null)
        { 
            stats.transform.GetChild(3).GetComponent<Text>().text = "Attack: " + tempEquipment.damageModifier.ToString();
            stats.transform.GetChild(4).GetComponent<Text>().text = "Health: " + tempEquipment.healthModifier.ToString();
            stats.transform.GetChild(5).GetComponent<Text>().text = "Attack Speed: " + tempEquipment.attackSpeedModifier.ToString();
            stats.transform.GetChild(6).GetComponent<Text>().text = tempEquipment.equipmentSlot.ToString();
        }
        else
        {
            stats.transform.GetChild(3).GetComponent<Text>().text = "";
            stats.transform.GetChild(4).GetComponent<Text>().text = "";
            stats.transform.GetChild(5).GetComponent<Text>().text = "";
            stats.transform.GetChild(6).GetComponent<Text>().text = "";
        }
        yield return new WaitForSeconds(0.5f);
    }
}
