using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentStatsUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        stats.transform.GetChild(0).GetComponent<Text>().text = slot.GetComponent<EquipSlot>().equipment.name;
        stats.transform.GetChild(1).GetComponent<Text>().text = slot.GetComponent<EquipSlot>().equipment.rarity.ToString();
        stats.transform.GetChild(2).GetComponent<Text>().text = slot.GetComponent<EquipSlot>().equipment.description;       
        stats.transform.GetChild(3).GetComponent<Text>().text = "Attack: " + slot.GetComponent<EquipSlot>().equipment.damageModifier.ToString();
        stats.transform.GetChild(4).GetComponent<Text>().text = "Health: " + slot.GetComponent<EquipSlot>().equipment.healthModifier.ToString();
        stats.transform.GetChild(5).GetComponent<Text>().text = "Attack Speed: " + slot.GetComponent<EquipSlot>().equipment.attackSpeedModifier.ToString();
        stats.transform.GetChild(6).GetComponent<Text>().text = slot.GetComponent<EquipSlot>().equipment.equipmentSlot.ToString();
       
        yield return new WaitForSeconds(0.5f);
    }
}
